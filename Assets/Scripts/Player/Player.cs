﻿using System.Collections;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Player class. There must be 
    /// 'PlayerVehicle' and 'PlayerDamageReceiver'
    /// as child objects
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    class Player : MonoBehaviour, IDamageable
    {
        #region constants
        /// <summary>
        /// Min health when regeneration without medkit can be applied 
        /// </summary>
        const float                 MinHealthForRegeneration = 20;

        /// <summary>
        /// Min health there must be after regeneration
        /// </summary>
        const float                 MinRegeneratedHealth = 20;
        /// <summary>
        /// How many health points will be regenerated without medkit
        /// </summary>
        const float                 HealthToRegenerate = 10;
        const float                 HealthAfterMedkit = 100;
        public const float          MaxHealth = 100;
        #endregion

        ISteeringWheel              steeringWheel;
        WeaponsController           weaponsController;
        GameScore                   currentScore;

        public Camera               MainCamera { get; private set; }
        public PlayerInventory      Inventory { get; private set; }
        public PlayerState          State { get; private set; }
        public float                Health { get; private set; } = MaxHealth;
        public GameScore            CurrentScore => currentScore;
        public PlayerVehicle        Vehicle { get; private set; }

        public event FloatChange    OnHealthChange;
        public event ScoreChange    OnScoreChange;
        public event PlayerDeath    OnPlayerDeath;
        public event PlayerStateChange OnPlayerStateChange;

        #region init / destroy
        /// <summary>
        /// Init player. 'PlayerVehicle' depends
        /// on 'IBackgroundController'
        /// </summary>
        public void Init(IBackgroundController background)
        {
            MainCamera = GetComponentInChildren<Camera>();

            Vehicle = GetComponentInChildren<PlayerVehicle>(true);
            Debug.Assert(Vehicle != null, "There must be a 'PlayerVehicle' as child object", this);

            Vehicle.Init(this);
            steeringWheel = Vehicle.SteeringWheel;

            GetComponentInChildren<HandsController>(true).Init();

            // reset score
            currentScore = new GameScore(Vehicle.MaxHealth);

            // sign to events
            Enemies.EnemyVehicle.OnEnemyDeath += AddEnemyScore;
            Enemies.EnemyVehicle.OnVehicleDestroy += AddEnemyVehicleScore;
            UI.InputController.OnHealthRegenerate += RegenerateHealth;
            Vehicle.OnVehicleCollision += CollideVehicle;

            State = PlayerState.Ready;
        }

        /// <summary>
        /// Inits inventory and weapons
        /// </summary>
        public void InitInventory()
        {
            Inventory = new PlayerInventory();

            weaponsController = GetComponentInChildren<WeaponsController>();
            weaponsController.Init(this);
        }

        /// <summary>
        /// To enable GC
        /// </summary>
        void UnsignFromEvents()
        {
            Enemies.EnemyVehicle.OnEnemyDeath -= AddEnemyScore;
            Enemies.EnemyVehicle.OnVehicleDestroy -= AddEnemyVehicleScore;
            UI.InputController.OnHealthRegenerate -= RegenerateHealth;
            Vehicle.OnVehicleCollision -= CollideVehicle;
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }
        #endregion

        public void UpdateInput(float horizonalAxis)
        {
            // if regenerating, still can steer
            if (State == PlayerState.Dead || State == PlayerState.Nothing)
            {
                return;
            }

            // player must call steering wheel methods
            // to control vehicle
            steeringWheel.Steer(horizonalAxis);
        }

        void AddEnemyScore(Enemies.EnemyData data)
        {
            currentScore.KillsAmount++;
            currentScore.KillsScore += data.Score;

            OnScoreChange(currentScore);
        }

        void AddEnemyVehicleScore(Enemies.EnemyVehicleData data)
        {
            currentScore.DestroyedVehiclesAmount++;
            currentScore.KillsScore += data.Score;

            OnScoreChange(currentScore);
        }

        #region health management
        void Die()
        {
            // ignore if already died
            if (State == PlayerState.Dead)
            {
                return;
            }

            State = PlayerState.Dead;
            OnPlayerStateChange(State);

            // send player's score
            currentScore.VehicleHealth = (int)Vehicle.Health;
            OnPlayerDeath(CurrentScore);

            // TODO:
            // sound
            CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Death);
        }

        public void RegenerateHealth()
        {            
            // if player is busy
            if (State != PlayerState.Ready)
            {
                return;
            }

            // if weapons controller is busy
            if (weaponsController.IsBusy())
            {
                return;
            }

            // regenerate if health is not max
            if (Health < MaxHealth)
            {
                StartCoroutine(WaitForRegeneration());
            }
        }

        IEnumerator WaitForRegeneration()
        {
            State = PlayerState.Regenerating;

            // weapons must be hidden,
            // so wait
            while (weaponsController.IsBusy())
            {
                yield return null;
            }

            // TODO:
            // start anim and sound

            Debug.Log("Regenerating health");

            float animLength = 1.0f;
            float waited = 0.0f;

            do
            {
                yield return null;
                waited += Time.deltaTime;

                // if died while regenerating
                if (State == PlayerState.Dead || State == PlayerState.Nothing)
                {
                    yield break;
                }

            } while (waited < animLength);

            // add health
            if (Health < MinHealthForRegeneration)
            {
                Health += HealthToRegenerate;

                if (Health < MinRegeneratedHealth)
                {
                    Health = MinRegeneratedHealth;
                }
            }
            else
            {
                Health = HealthAfterMedkit;
            }

            State = PlayerState.Ready;
        }


        /// <summary>
        /// Note: must be called only by 'PlayerDamageReceiver'
        /// </summary>
        public void ReceiveDamage(Damage damage)
        {
            if (Health <= 0)
            {
                return;
            }

            float damageValue = damage.CalculateDamageValue(transform.position);

            if (damageValue > 0)
            {
                Health -= damageValue;
                OnHealthChange(Health);

                if (Health <= 0)
                {
                    // normalize
                    Health = 0;

                    Die();
                }
                else
                {
                    if (damage.Type == DamageType.Explosion)
                    {
                        CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Explosion);
                    }
                    else
                    {
                        CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Damage);
                    }
                }
            }
        }
        #endregion

        void CollideVehicle(IVehicle other, float damage)
        {
            // reduce full damage
            float damageMultiplier = 0.5f * MaxHealth / Vehicle.MaxHealth;

            damage *= damageMultiplier;

            if (damage > 0)
            {
                // receive damage
                ReceiveDamage(Damage.CreateBulletDamage(
                    damage, -transform.forward, transform.position, transform.up, null));

                if (Health > 0)
                {
                    // if still alive, play default animation
                    CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Collision);
                }
                // otherwise death animation will be played (in receive damage)
            }
            else
            {
                // just play animation
                CameraShaker.Instance.PlayAnimation(CameraShaker.CameraAnimation.Collision);
            }
        }
    }
}

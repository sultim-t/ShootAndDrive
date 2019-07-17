﻿using System.Collections;
using UnityEngine;
using SD.Weapons;

namespace SD.PlayerLogic
{
    delegate void FloatChange(float f);
    delegate void PlayerScore(GameScore score);

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

        PlayerVehicle           playerVehicle;
        ISteeringWheel          steeringWheel;
        WeaponsController       weaponsController;
        GameScore               currentScore;

        public Camera           MainCamera { get; private set; }
        public PlayerInventory  Inventory { get; private set; }
        public PlayerState      State { get; private set; }
        public float            Health { get; private set; } = MaxHealth;
        public GameScore        CurrentScore => currentScore;
        public PlayerVehicle    Vehicle => playerVehicle;

        public event FloatChange OnHealthChange;
        public event PlayerScore OnPlayerDeath;

        /// <summary>
        /// Init player. 'PlayerVehicle' depends
        /// on 'IBackgroundController'
        /// </summary>
        public void Init(IBackgroundController background)
        {
            MainCamera = GetComponentInChildren<Camera>();

            playerVehicle = GetComponentInChildren<PlayerVehicle>(true);
            Debug.Assert(playerVehicle != null, "There must be a 'PlayerVehicle' as child object", this);

            playerVehicle.Init(this, background);
            steeringWheel = playerVehicle.SteeringWheel;

            // reset score
            currentScore = new GameScore(PlayerVehicle.MaxHealth);

            // sign to events
            Enemies.EnemyVehicle.OnEnemyDeath += AddEnemyScore;
            Enemies.EnemyVehicle.OnVehicleDestroy += AddEnemyVehicleScore;
            UI.InputController.OnHealthRegenerate += RegenerateHealth;

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
        }

        void OnDestroy()
        {
            UnsignFromEvents();
        }

        public void UpdateInput(float horizonalAxis)
        {
            // player must call steering wheel methods
            // to control vehicle
            steeringWheel.Steer(horizonalAxis);
        }

        void AddEnemyScore(Enemies.EnemyData data)
        {
            currentScore.KillsAmount++;
            currentScore.KillsScore += data.Score;
        }

        void AddEnemyVehicleScore(Enemies.EnemyVehicleData data)
        {
            currentScore.DestroyedVehiclesAmount++;
            currentScore.KillsScore += data.Score;
        }

        #region health management
        void Die()
        {
            State = PlayerState.Dead;

            // send player's score
            currentScore.VehicleHealth = (int)playerVehicle.Health;
            OnPlayerDeath(CurrentScore);

            // TODO:
            // play anim, sound

            // hide weapon
            weaponsController.HideWeapon();
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

            // hide weapon and wait
            weaponsController.HideWeapon();
            while (weaponsController.IsBusy())
            {
                yield return null;
            }

            // TODO:
            // anim, sound

            do
            {
                yield return null;

                // if died while regenerating
                if (State == PlayerState.Dead || State == PlayerState.Nothing)
                {
                    yield break;
                }

            } while (State != PlayerState.Ready);

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

            // finally, take out weapon
            weaponsController.TakeOutWeapon();

            State = PlayerState.Ready;
        }
#endregion

#region inherited
        /// <summary>
        /// Note: must be called only by 'PlayerDamageReceiver'
        /// </summary>
        public void ReceiveDamage(Damage damage)
        {
            float damageValue = damage.Value;

            if (damage.Type == DamageType.Explosion)
            {
                float length = (transform.position - damage.Point).magnitude;
                
                if (length < 1.0f)
                {
                    length = 1.0f;
                }

                damageValue /= length;
            }

            Health -= damageValue;
            OnHealthChange(Health);

            if (Health <= 0)
            {
                Die();
            }
        }
#endregion
    }
}

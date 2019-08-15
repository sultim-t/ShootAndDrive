﻿using SD.Weapons;

namespace SD.PlayerLogic
{
    // Represents weapon item in player's inventory
    class WeaponItem
    {
        // fields
        public bool         IsBought;
        public RefInt       HealthRef { get; }
        public WeaponIndex  This { get; }

        // getters
        public bool         IsBroken => HealthRef.Value <= 0;
        public WeaponData   Stats => AllWeaponsStats.Instance[This];

        public WeaponItem(WeaponIndex weapon, int health, bool isBought)
        {
            this.This = weapon;
            this.HealthRef = new RefInt(health);
            this.IsBought = isBought;
        }
    }
}

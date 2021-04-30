using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Musashi
{
    [CustomEditor(typeof(WeaponSettingSOData))]
    public class WeaponDataCustomInspector : Editor
    {
        WeaponSettingSOData weaponSetting;


        private void OnEnable()
        {
            weaponSetting = target as WeaponSettingSOData;    
        }

        public override void OnInspectorGUI()
        {
            //weaponSetting = target as WeaponSettingSOData;

            DrawGeneral();

            DrawGunSetting();

            EditorUtility.SetDirty(weaponSetting);
        }


        private void DrawGeneral()
        {
            GUILayout.Label("General settings", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label("Weapon name");
            weaponSetting.weaponName = EditorGUILayout.TextArea(weaponSetting.weaponName);
            GUILayout.Label("Weapon type");
            weaponSetting.weaponType = (WeaponType)EditorGUILayout.EnumPopup(weaponSetting.weaponType);
            GUILayout.EndVertical();
        }

        private void DrawGunSetting()
        {
            GUILayout.Label("Gun settings", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            //Main
            GUILayout.Label("Main settings", EditorStyles.centeredGreyMiniLabel);
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Shot damage");
            weaponSetting.shotDamage = EditorGUILayout.FloatField(weaponSetting.shotDamage);
            GUILayout.Label("Shot power");
            weaponSetting.shotPower = EditorGUILayout.FloatField(weaponSetting.shotPower);
            GUILayout.Label("Fire rate");
            weaponSetting.fireRate = EditorGUILayout.FloatField(weaponSetting.fireRate);
            GUILayout.Label("Aim camera fov");
            weaponSetting.aimCameraFOV = EditorGUILayout.FloatField(weaponSetting.aimCameraFOV);
            GUILayout.Label("Aim camera speed");
            weaponSetting.aimSpeed = EditorGUILayout.FloatField(weaponSetting.aimSpeed);
            GUILayout.Label("Recoil Setting");
            weaponSetting.recoil = EditorGUILayout.Vector2Field("Recoil", weaponSetting.recoil);
            GUILayout.Label("Ammo and MuzzleFlash Pool size");
            weaponSetting.ammoAndMuzzleFlashPoolSize = EditorGUILayout.IntSlider(weaponSetting.ammoAndMuzzleFlashPoolSize, 1, 100);
            GUILayout.EndVertical();
            //Effect
            GUILayout.Label("Effects", EditorStyles.centeredGreyMiniLabel);
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("MuzzleFlash effect");
            weaponSetting.muzzleFlash = (ParticleSystem)EditorGUILayout.ObjectField(weaponSetting.muzzleFlash, typeof(ParticleSystem), false);
            GUILayout.Label("Shot sound effect");
            weaponSetting.shotSFX = (AudioClip)EditorGUILayout.ObjectField(weaponSetting.shotSFX, typeof(AudioClip), false);
            GUILayout.Label("Reloading sound effect");
            weaponSetting.reloadSFX = (AudioClip)EditorGUILayout.ObjectField(weaponSetting.reloadSFX, typeof(AudioClip), false);
            GUILayout.Label("Weapon empty sound effect");
            weaponSetting.emptySFX = (AudioClip)EditorGUILayout.ObjectField(weaponSetting.emptySFX, typeof(AudioClip), false);
            if (weaponSetting.weaponType == WeaponType.ShotGun)
            {
                GUILayout.Label("Shotgun cartridge loading clip");
                weaponSetting.shotgunLoadingSFX = (AudioClip)EditorGUILayout.ObjectField(weaponSetting.shotgunLoadingSFX, typeof(AudioClip), false);
            }
            GUILayout.EndVertical();
            //Objcets
            GUILayout.Label("Ammo settings", EditorStyles.centeredGreyMiniLabel);
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Bullet prefab");
            weaponSetting.bullet = (BulletControl)EditorGUILayout.ObjectField(weaponSetting.bullet, typeof(BulletControl), false);
            GUILayout.Label("Max ammo number");
            weaponSetting.maxAmmo = EditorGUILayout.IntField(weaponSetting.maxAmmo);
            GUILayout.EndVertical();

            GUILayout.EndVertical();

        }
    }
}

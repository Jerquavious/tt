using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GorillaLocomotion;
using GorillaNetworking;
using easyInputs;
using UnhollowerRuntimeLib;
using Photon.Pun;
using Util;

namespace HandHeldTemplate.Mods
{
    class ModManager
    {
        public static GameObject rightHandPlatform { get; private set; }
        public static GameObject leftHandPlatform { get; private set; }
        public static bool rightPlatSpawned = false;
        public static bool leftPlatSpawned = false;
        private static float platformCooldown = 0f;
        private static float platformCooldown2 = 0f;
        private static GameObject activePointer = null;
        private static LineRenderer sharedLineRenderer = null;
        private static float holdDuration = 0.2f;
        private static float holdTimer = 0f;
        private static float flyActivationDelay = 0.2f;
        private static float flyHoldTimer = 0f;
        private static bool buttonDownLastFrame = false;

        public static void Fly(int speed)
        {
            if (EasyInputs.GetPrimaryButtonDown(EasyHand.RightHand))
            {
                flyHoldTimer += Time.deltaTime;
                if (flyHoldTimer >= flyActivationDelay)
                {
                    GorillaLocomotion.Player.Instance.playerRigidBody.velocity = Vector3.zero;
                    GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * speed * Time.deltaTime;
                }
            }
            else
            {
                flyHoldTimer = 0f;
            }
        }

        public static void SpeedBoost()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaTagManager.instance.slowJumpLimit = 8f;
                GorillaTagManager.instance.slowJumpMultiplier = 1.4f;
            }
        }

        public static void Platforms(Color color)
        {
            bool gripButtonDown = EasyInputs.GetGripButtonDown(EasyHand.LeftHand);
            bool gripButtonDown2 = EasyInputs.GetGripButtonDown(EasyHand.RightHand);
            if (platformCooldown == 0f && gripButtonDown2)
            {
                platformCooldown = 1f;
                rightHandPlatform = new GameObject();
                rightHandPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightHandPlatform.GetComponent<Renderer>().material.SetColor("_Color", color);
                UnityEngine.Object.Destroy(rightHandPlatform.GetComponent<Rigidbody>());
                rightHandPlatform.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f);
                rightHandPlatform.transform.position = new Vector3(0f, -0.0075f, 0f) + Player.Instance.rightHandTransform.position;
                rightHandPlatform.transform.rotation = Player.Instance.rightHandTransform.rotation;
            }
            else if (platformCooldown == 1f && !gripButtonDown2)
            {
                platformCooldown = 0f;
                UnityEngine.Object.DestroyImmediate(rightHandPlatform);
                UnityEngine.Object.DestroyImmediate(rightHandPlatform.gameObject);
                UnityEngine.Object.Destroy(rightHandPlatform);
                UnityEngine.Object.Destroy(rightHandPlatform.gameObject);
                rightHandPlatform = null;
            }
            if (platformCooldown2 == 0f && gripButtonDown)
            {
                platformCooldown2 = 1f;
                leftHandPlatform = new GameObject();
                leftHandPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftHandPlatform.GetComponent<Renderer>().material.SetColor("_Color", color);
                UnityEngine.Object.Destroy(leftHandPlatform.GetComponent<Rigidbody>());
                leftHandPlatform.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f);
                leftHandPlatform.transform.position = new Vector3(0f, -0.0075f, 0f) + Player.Instance.leftHandTransform.position;
                leftHandPlatform.transform.rotation = Player.Instance.leftHandTransform.rotation;
                return;
            }
            if (platformCooldown2 == 1f && !gripButtonDown)
            {
                platformCooldown2 = 0f;
                UnityEngine.Object.DestroyImmediate(leftHandPlatform);
                UnityEngine.Object.DestroyImmediate(leftHandPlatform.gameObject);
                UnityEngine.Object.Destroy(leftHandPlatform);
                UnityEngine.Object.Destroy(leftHandPlatform.gameObject);
                leftHandPlatform = null;
            }
        }

        public static void NoClip()
        {
            bool secondaryButtonDownCurrentFrame = EasyInputs.GetSecondaryButtonDown(EasyHand.RightHand);

            if (secondaryButtonDownCurrentFrame)
            {
                holdTimer += Time.deltaTime;
            }
            else
            {
                holdTimer = 0f;
            }

            if (holdTimer >= holdDuration && buttonDownLastFrame)
            {
                var colliders = Resources.FindObjectsOfTypeAll<Collider>();

                foreach (var collider in colliders)
                {
                    collider.enabled = !secondaryButtonDownCurrentFrame || !collider.enabled;
                }
                holdTimer = 0f;
            }

            buttonDownLastFrame = secondaryButtonDownCurrentFrame;
        }

        public static void AllCosmetics()
        {
            foreach (CosmeticsController.CosmeticItem itemToBuy in CosmeticsController.instance.allCosmetics)
            {
                if (CosmeticsController.instance != null)
                {
                    CosmeticsController.instance.itemToBuy = itemToBuy;
                    CosmeticsController.instance.PurchaseItem();
                    CosmeticsController.instance.unlockedCosmetics = CosmeticsController.instance.allCosmetics;
                }
            }
        }

        public static void TagGunMaster()
        {
            RaycastHit raycastHit;
            Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit);

            InteractivePointer.Initialize();

            InteractivePointer.UpdatePointerPosition(raycastHit.point);

            InteractivePointer.SetLineRendererPositions(
                GorillaLocomotion.Player.Instance.rightHandTransform.transform.position,
                raycastHit.point
            );

            List<Component> hitComponents = InteractivePointer.GetHitComponents(raycastHit);

            if (!PhotonNetwork.IsMasterClient && EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
            {
                return;
            }

            if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) && hitComponents.Any(comp => comp is PhotonView))
            {
                PhotonView photonView = hitComponents.FirstOrDefault(comp => comp is PhotonView) as PhotonView;

                if (photonView != null)
                {
                    Photon.Realtime.Player owner = photonView.Owner;
                    foreach (GorillaTagManager gorillaTagManager in UnityEngine.Object.FindObjectsOfType<GorillaTagManager>())
                    {
                        gorillaTagManager.AddInfectedPlayer(owner);
                    }
                }
            }
        }

        public static void TagGunNoMaster()
        {
            RaycastHit raycastHit;
            Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.position, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit);

            InteractivePointer.Initialize();

            InteractivePointer.UpdatePointerPosition(raycastHit.point);

            InteractivePointer.SetLineRendererPositions(
                GorillaLocomotion.Player.Instance.rightHandTransform.transform.position,
                raycastHit.point
            );

            if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) && raycastHit.collider != null)
            {
                Player.Instance.rightHandTransform.position = raycastHit.point;
            }
        }
    }
}

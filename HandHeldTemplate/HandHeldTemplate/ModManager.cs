using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GorillaLocomotion;
using easyInputs;
using Photon.Pun;
using Util;
using UnityEngine.EventSystems;
using GorillaNetworking;
using UnityEngine.AI;

namespace HandHeldTemplate.Mods
{
    class ModManager
    {
        public static GameObject pointerFreeze;
        public static GameObject pointerCrash;
        public static GameObject pointerKick;
        public static GameObject projectile;
        public static GameObject sphereLGhost;
        public static GameObject sphereRGhost;
        public static GameObject sphereLInvis;
        public static GameObject sphereRInvis;
        public static GameObject rightPlat;
        public static GameObject leftPlat;
        public static GameObject cube;
        public static bool lockEnabled;
        public static bool rightPlatEnabled;
        public static bool leftPlatEnabled;
        public static string taggerSound;
        public static float fixedtagger;
        public static string rapid;
        public static float fixedrapid;
        public static int rape = 0;
        public static float speedWatcher;
        public static VRRig actualrig;
        public static System.Collections.Generic.List<Photon.Realtime.Player> staycrasheduser = new System.Collections.Generic.List<Photon.Realtime.Player>();
        private static GameObject activePointer = null;
        private static LineRenderer sharedLineRenderer = null;
        private static float holdDuration = 0f;
        private static float holdTimer = 0f;
        private static float flyActivationDelay = 0f;
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

        public static void MosaSpeed()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaTagManager.instance.slowJumpLimit = 8.5f;
                GorillaTagManager.instance.slowJumpMultiplier = 1.5f;
            }
        }

        public static void FastSpeedBoost()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaTagManager.instance.slowJumpLimit = 9.5f;
                GorillaTagManager.instance.slowJumpMultiplier = 1.8f;
            }
        }

        public static void FasterSpeedBoost()
        {
            if (PhotonNetwork.InRoom)
            {
                GorillaTagManager.instance.slowJumpLimit = 10.5f;
                GorillaTagManager.instance.slowJumpMultiplier = 2.0f;
            }
        }

        public static void PlaceHolder()
        {
            // Placeholder
        }

        private static bool isLongArmsEnabled = false;

        public static void ToggleLongArms()
        {
            isLongArmsEnabled = !isLongArmsEnabled;

            if (isLongArmsEnabled)
            {
                GameObject.Find("GorillaPlayer").transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                GameObject.Find("GorillaPlayer").transform.localScale = new Vector3(1f, 1f, 1f);
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

        public static void TagGunMaster()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
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
                GorillaLocomotion.Player.Instance.rightHandTransform.position = raycastHit.point;
            }
        }

        public static void Disconnect()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.Disconnect();
            }
        }

        public static void Crash()
        {
            if (EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand))
            {
                PhotonNetwork.DestroyAll();
            }
        }

        public static void InvisMonke()
        {
            if (EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand) && GorillaTagger.Instance.myVRRig != null)
            {
                GorillaTagger.Instance.myVRRig.enabled = false;
                GorillaTagger.Instance.myVRRig.transform.position = new Vector3(999f, 999f, 999f);
                return;
            }
            GorillaTagger.Instance.myVRRig.enabled = true;
        }

        public static void GhostMonke()
        {
            if (EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand) && GorillaTagger.Instance.myVRRig != null)
            {
                GorillaTagger.Instance.myVRRig.enabled = false;
                return;
            }
            GorillaTagger.Instance.myVRRig.enabled = true;
        }

        public static void Platforms()
        {
            if (EasyInputs.GetGripButtonDown(EasyHand.LeftHand))
            {
                if (!leftPlatEnabled)
                {
                    leftPlat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leftPlat.GetComponent<Renderer>().material.color = Color.white;
                    leftPlat.transform.localScale = new Vector3(0.25f, 0.018f, 0.30f);
                    leftPlat.transform.position = GorillaLocomotion.Player.Instance.leftHandTransform.position + new Vector3(0f, -0.02f, 0f);
                    leftPlat.transform.rotation = GorillaLocomotion.Player.Instance.leftHandTransform.rotation * Quaternion.Euler(0f, 0f, -90f);
                    leftPlatEnabled = true;
                }
            }
            else
            {
                if (leftPlatEnabled)
                {
                    UnityEngine.Object.Destroy(leftPlat);
                    leftPlatEnabled = false;
                    return;
                }
            }
            if (EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                if (!rightPlatEnabled)
                {
                    rightPlat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rightPlat.GetComponent<Renderer>().material.color = Color.white;
                    rightPlat.transform.localScale = new Vector3(0.25f, 0.018f, 0.30f);
                    rightPlat.transform.position = GorillaLocomotion.Player.Instance.rightHandTransform.position + new Vector3(0f, -0.02f, 0f);
                    rightPlat.transform.rotation = GorillaLocomotion.Player.Instance.rightHandTransform.rotation * Quaternion.Euler(0f, 0f, -90f);
                    rightPlatEnabled = true;
                }
            }
            else
            {
                if (rightPlatEnabled)
                {
                    UnityEngine.Object.Destroy(rightPlat);
                    rightPlatEnabled = false;
                }
            }
        }

        public static void SetMaster()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        public static void LeftDisconnect()
        {
            if (EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand))
            {
                PhotonNetwork.Disconnect();
            }
        }

        public static void RightDisconnect()
        {
            if (EasyInputs.GetPrimaryButtonDown(EasyHand.RightHand))
            {
                PhotonNetwork.Disconnect();
            }
        }


        public static void NoName()
        {
            SetName(" ");
        }

        private static void SetName(string PlayerName)
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            PhotonNetwork.NickName = PlayerName;
            PhotonNetwork.NetworkingClient.NickName = PlayerName;
            GorillaComputer.instance.currentName = PlayerName;
            GorillaComputer.instance.savedName = PlayerName;
            GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
            GorillaLocomotion.Player.Instance.name = PlayerName;
            PlayerPrefs.SetString("playerName", PlayerName);
            PlayerPrefs.Save();

        }

        public static void AntiTagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = false;
        }

        public static void TagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = true;
        }

        public static void GrabDoug()
        {
            GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }
        public static void GrabBat()
        {
            GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }
        public static void TagAll()
        {
            GorillaTagger.Instance.sphereCastRadius = 999;
        }

        public static void FarTagAura()
        {
            GorillaTagger.Instance.sphereCastRadius = 10;
        }
        public static void TagAura()
        {
            GorillaTagger.Instance.sphereCastRadius = 5;
        }
        public static void NoTagCoolDown()
        {
            GorillaTagger.Instance.tagCooldown = 0;
        }
        public static void LoudHits()
        {
            GorillaTagger.Instance.handTapVolume = 999f;
        }

        public static void NoHits()
        {
            GorillaTagger.Instance.handTapVolume = 0f;
        }

        public static void MatAll()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            foreach (GorillaTagManager gorillaTagManager in UnityEngine.Object.FindObjectsOfType<GorillaTagManager>())
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    if (!gorillaTagManager.photonView.IsMine)
                    {
                        gorillaTagManager.photonView.RequestOwnership();
                    }
                    if (gorillaTagManager.photonView.IsMine)
                    {
                        GorillaLocomotion.Player.Instance.disableMovement = false;
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                        gorillaTagManager.currentInfected.Remove(player);
                        gorillaTagManager.AddInfectedPlayer(player);
                    }
                }
            }
        }
        public static void PillowSpam()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
                {
            4,
            true,
            2f
                });
        }

        public static void Earrape()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            UnityEngine.Random.Range(1, 20),
            true,
            2f
                });
        }

        public static void EarrapeV2()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            UnityEngine.Random.Range(20, 40),
            true,
            2f
                });
        }
        public static void EarrapeV3()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            UnityEngine.Random.Range(40, 60),
            true,
            2f
                });

        }

        public static void HandTapSpam()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            2,
            true,
            2f
                });
        }

        public static void GlassSpam()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            43,
            true,
            2f
                });
        }

        public static void MetalSpam()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, (Il2CppSystem.Object[])new object[]
            {
            26,
            true,
            2f
                });
        }
        public static void VibrateAll()
        {
            if (EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                foreach (VRRig vrrigs in UnityEngine.Object.FindObjectsOfType<VRRig>())
                {
                    PhotonView photonView = vrrigs.GetComponent<PhotonView>();
                    if (photonView != GorillaTagger.Instance.myVRRig.photonView)
                    {
                        photonView.RPC("SetJoinTaggedTime", RpcTarget.Others, null);
                    }
                }
            }
        }
        public static void Helicopter()
        {
            if (EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand))
            {
                GorillaTagger.Instance.myVRRig.enabled = false;
                GorillaTagger.Instance.myVRRig.transform.Rotate(0f, 7f, 0f);
                GorillaTagger.Instance.myVRRig.transform.position += Vector3.up * Time.deltaTime * 0.9f;
                return;
            }
            else
            {
                GorillaTagger.Instance.myVRRig.enabled = true;

            }
        }
        public static void PCMonkey()
        {
            GorillaLocomotion.Player.Instance.Turn(EasyInputs.GetThumbStick2DAxis(EasyHand.RightHand).x * 3f);
            GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * EasyInputs.GetThumbStick2DAxis(EasyHand.LeftHand).y * 7f;
            GorillaLocomotion.Player.Instance.playerRigidBody.useGravity = true;
            if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
            {
                GorillaLocomotion.Player.Instance.playerRigidBody.useGravity = false;
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.up * Time.deltaTime * 7f;
            }
            if (EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand))
            {
                GorillaLocomotion.Player.Instance.playerRigidBody.useGravity = false;
            }
            if (PhotonNetwork.InRoom)
            {
                GorillaTagger.Instance.myVRRig.head.rigTarget.localEulerAngles = Vector3.right;
                GorillaTagger.Instance.myVRRig.leftHand.rigTarget.localPosition = GorillaTagger.Instance.myVRRig.head.rigTarget.localPosition;
                GorillaTagger.Instance.myVRRig.rightHand.rigTarget.localPosition = GorillaTagger.Instance.myVRRig.head.rigTarget.localPosition;
                GorillaTagger.Instance.myVRRig.leftHand.rigTarget.localEulerAngles = Vector3.zero;
                GorillaTagger.Instance.myVRRig.rightHand.rigTarget.localEulerAngles = Vector3.zero;
            }
        }
        public static void SizeChanger()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            float scale = 1.0f;
            if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
            {
                scale++;
                if (!PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.offlineVRRig.transform.localScale = new Vector3(scale, scale, scale);
                }
                else if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.transform.localScale = new Vector3(scale, scale, scale);
                }
            }

            if (EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand))
            {
                scale--;
                if (!PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.offlineVRRig.transform.localScale = new Vector3(scale, scale, scale);
                }
                else if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
        public static void CopyMovementsGun()
        {
            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerCrash);
            }
            else
            {
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerCrash == null;
                if (flag3)
                {
                    pointerCrash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerCrash.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerCrash.GetComponent<SphereCollider>());
                    pointerCrash.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                VRRig ownerrig = raycastHit.collider.GetComponentInParent<VRRig>();
                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerCrash.transform.position = raycastHit.point;
                }
                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) && !ownerrig.photonView.IsMine)
                {
                    pointerCrash.GetComponent<Renderer>().material.color = Color.red;
                    actualrig = ownerrig;
                    pointerCrash.transform.position = actualrig.gameObject.transform.position;
                    while (EasyInputs.GetSecondaryButtonDown(EasyHand.RightHand))
                    {
                        pointerCrash.transform.position = actualrig.gameObject.transform.position;
                    }
                    GorillaTagger.Instance.myVRRig.enabled = false;
                    GorillaTagger.Instance.myVRRig.rightHand.rigTarget.position = actualrig.rightHand.rigTarget.position;
                    GorillaTagger.Instance.myVRRig.leftHand.rigTarget.position = actualrig.leftHand.rigTarget.position;
                    GorillaTagger.Instance.myVRRig.leftHand.rigTarget.rotation = actualrig.rightHand.rigTarget.rotation;
                    GorillaTagger.Instance.myVRRig.leftHand.rigTarget.rotation = actualrig.leftHand.rigTarget.rotation;
                    GorillaTagger.Instance.myVRRig.head.rigTarget.transform.rotation = actualrig.head.rigTarget.transform.rotation;
                    GorillaTagger.Instance.myVRRig.transform.rotation = actualrig.transform.rotation;
                    GorillaTagger.Instance.myVRRig.transform.position = actualrig.transform.position;
                }

                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerCrash.GetComponent<Renderer>().material.color = Color.white;
                    GorillaTagger.Instance.myVRRig.enabled = true;
                }
            }
        }

        public static void Wait(float waitTime)
        {
            speedWatcher += Time.deltaTime;
            if (speedWatcher >= waitTime)
            {
                speedWatcher = 0.0f;
                return;
            }
        }


        public static void RapeGun()
        {
            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerCrash);
            }
            else
            {
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerCrash == null;
                if (flag3)
                {
                    pointerCrash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerCrash.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerCrash.GetComponent<SphereCollider>());
                    pointerCrash.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                VRRig ownerrig = raycastHit.collider.GetComponentInParent<VRRig>();
                pointerCrash.transform.position = raycastHit.point;
                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) && !ownerrig.photonView.IsMine)
                {
                    pointerCrash.GetComponent<Renderer>().material.color = Color.red;
                    GorillaTagger.Instance.myVRRig.enabled = false;
                    if (rape == 0)
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = ownerrig.transform.position - ownerrig.transform.forward * 1f;
                        GorillaTagger.Instance.myVRRig.transform.rotation = ownerrig.transform.rotation;
                        Wait(2f);
                        rape = 1;
                        return;
                    }
                    if (rape == 1)
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = ownerrig.transform.position - ownerrig.transform.forward * 0.5f;
                        GorillaTagger.Instance.myVRRig.transform.rotation = ownerrig.transform.rotation;
                        Wait(2f);
                        rape = 2;
                        return;
                    }
                    if (rape == 2)
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = ownerrig.transform.position - ownerrig.transform.forward * 0.35f;
                        GorillaTagger.Instance.myVRRig.transform.rotation = ownerrig.transform.rotation;
                        Wait(2f);
                        rape = 0;
                        return;
                    }
                }

                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerCrash.GetComponent<Renderer>().material.color = Color.white;
                    GorillaTagger.Instance.myVRRig.enabled = true;
                }
            }
        }
        public static void JumpscareGun()
        {
            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerFreeze);
            }
            else
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerFreeze == null;

                if (flag3)
                {
                    pointerFreeze = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerFreeze.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerFreeze.GetComponent<SphereCollider>());
                    pointerFreeze.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }

                VRRig owner = raycastHit.collider.GetComponentInParent<VRRig>();
                pointerFreeze.transform.position = raycastHit.point;
                NavMeshAgent[] navAgents2 = UnityEngine.Object.FindObjectsOfType<NavMeshAgent>();

                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    foreach (NavMeshAgent agent in navAgents2)
                    {

                        agent.gameObject.transform.position = owner.transform.position;
                        if (lockEnabled)
                        {
                            if (EasyInputs.GetSecondaryButtonDown(EasyHand.LeftHand))
                            {
                                pointerFreeze.transform.position = agent.gameObject.transform.position;
                            }
                            else
                            {
                                pointerFreeze.transform.position = raycastHit.point;
                            }
                        }
                    }
                }

                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }
            }
        }
        public static void RigGun()
        {
            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerFreeze);
            }
            else
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerFreeze == null;

                if (flag3)
                {
                    pointerFreeze = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerFreeze.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerFreeze.GetComponent<SphereCollider>());
                    pointerFreeze.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }

                pointerFreeze.transform.position = raycastHit.point;

                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    GorillaTagger.Instance.myVRRig.transform.position = pointerFreeze.transform.position;
                    GorillaTagger.Instance.myVRRig.enabled = false;
                }

                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }
            }
        }
        public static void FixTag()
        {
            GorillaTagger.Instance.sphereCastRadius = 1;
        }
        public static void GrabRig()
        {
            if (EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                GorillaTagger.Instance.myVRRig.enabled = false;
                GorillaTagger.Instance.myVRRig.transform.position = GorillaLocomotion.Player.Instance.rightHandTransform.transform.position;
            }
            else if (EasyInputs.GetGripButtonDown(EasyHand.LeftHand))
            {
                GorillaTagger.Instance.myVRRig.enabled = false;
                GorillaTagger.Instance.myVRRig.transform.position = GorillaLocomotion.Player.Instance.leftHandTransform.transform.position;
            }
        }
        public static void ReachGun()
        {

            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerKick);
            }
            else
            {
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerKick == null;
                if (flag3)
                {
                    pointerKick = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerKick.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerKick.GetComponent<SphereCollider>());
                    pointerKick.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointerKick.transform.position = raycastHit.point;
                Photon.Realtime.Player owner = raycastHit.collider.GetComponentInParent<PhotonView>().Owner;
                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerKick.GetComponent<Renderer>().material.color = Color.red;
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                    GorillaLocomotion.Player.Instance.maxArmLength = 999999999999;
                    GorillaLocomotion.Player.Instance.rightHandTransform.transform.position = pointerKick.transform.position;
                }
                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerKick.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }

        public static void MatGun()
        {
            if (!EasyInputs.GetGripButtonDown(EasyHand.RightHand))
            {
                UnityEngine.Object.Destroy(pointerKick);
            }
            else
            {
                RaycastHit raycastHit;
                bool flag3 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightHandTransform.transform.position - GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, -GorillaLocomotion.Player.Instance.rightHandTransform.transform.up, out raycastHit) && pointerKick == null;
                if (flag3)
                {
                    pointerKick = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(pointerKick.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(pointerKick.GetComponent<SphereCollider>());
                    pointerKick.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                pointerKick.transform.position = raycastHit.point;
                Photon.Realtime.Player owner = raycastHit.collider.GetComponentInParent<PhotonView>().Owner;
                if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerKick.GetComponent<Renderer>().material.color = Color.red;
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                    foreach (GorillaTagManager gorillaTagManager in UnityEngine.Object.FindObjectsOfType<GorillaTagManager>())
                    {
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                        gorillaTagManager.currentInfected.Add(owner);
                        gorillaTagManager.currentInfected.Remove(owner);
                    }
                }
                if (!EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
                {
                    pointerKick.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
       
        public static void TpGun()

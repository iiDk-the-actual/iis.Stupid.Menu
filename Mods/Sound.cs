using ExitGames.Client.Photon;
using iiMenu.Notifications;
using OVR;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    internal class Sound
    {
        public static void BetaPlayTag(int id, float volume)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                if (Time.time > soundDebounce)
                {
                    object[] soundSendData = new object[2];
                    soundSendData[0] = id;
                    soundSendData[1] = volume;

                    object[] sendEventData = new object[3];
                    sendEventData[0] = PhotonNetwork.ServerTimestamp;
                    sendEventData[1] = (byte)3;
                    sendEventData[2] = soundSendData;
                    try
                    {
                        PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
                    }
                    catch { /* wtf */ }
                    RPCProtection();

                    soundDebounce = Time.time + 0.2f;
                }
            }
        }

        public static void RandomSoundSpam()
        {
            if (rightGrab)
            {
                int soundId = UnityEngine.Random.Range(0, 259);
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
            }
        }

        public static void BassSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        68,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 999999f);
                }
            }
        }

        public static void MetalSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        18,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(18, false, 999999f);
                }
            }
        }

        public static void WolfSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        195,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(195, false, 999999f);
                }
            }
        }

        public static void CatSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        236,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(236, false, 999999f);
                }
            }
        }

        public static void TurkeySoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        83,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(83, false, 999999f);
                }
            }
        }

        public static void FrogSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        91,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(91, false, 999999f);
                }
            }
        }

        public static void BeeSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        191,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(191, false, 999999f);
                }
            }
        }

        public static void EarrapeSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        215,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(215, false, 999999f);
                }
            }
        }

        public static void DingSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        244,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(244, false, 999999f);
                }
            }
        }

        public static void CrystalSoundSpam()
        {
            if (rightGrab)
            {
                int[] sounds = new int[]
                {
                    UnityEngine.Random.Range(40,54),
                    UnityEngine.Random.Range(214,221)
                };
                int soundId = sounds[UnityEngine.Random.Range(0, 1)];
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
                RPCProtection();
            }
        }

        public static void BigCrystalSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        213,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(213, false, 999999f);
                }
            }
        }

        public static void PanSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        248,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(248, false, 999999f);
                }
            }
        }

        public static void AK47SoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        203,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(203, false, 999999f);
                }
            }
        }

        public static void SqueakSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        75 + (Time.frameCount % 2),
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(75 + (Time.frameCount % 2), false, 999999f);
                }
            }
        }

        public static void SirenSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        48 + ((Time.frameCount / 15) % 2) * 2,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(48 + ((Time.frameCount / 15) % 2) * 2, false, 999999f);
                }
            }
        }

        public static void DecreaseSoundID()
        {
            soundId = soundId - 1;
            if (soundId < 0)
            {
                soundId = 0;
            }
            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseSoundID()
        {
            soundId++;
            GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId.ToString() + "</color><color=grey>]</color>";
        }

        public static void CustomSoundSpam()
        {
            if (rightGrab)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        soundId,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundId, false, 999999f);
                }
            }
        }

        public static void CountSoundSpam()
        {
            if (rightGrab)
            {
                /*GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    1,
                    999999f
                });
                RPCProtection();*/
                BetaPlayTag(1, 999999f);
            }
        }

        public static void BrawlCountSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(6, 999999f);
            }
        }

        public static void BrawlStartSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(7, 999999f);
            }
        }

        public static void TagSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(0, 999999f);
            }
        }

        public static void RoundEndSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(2, 999999f);
            }
        }

        public static void BonkSoundSpam()
        {
            if (rightGrab)
            {
                BetaPlayTag(4, 999999f);
            }
        }
    }
}

using ExitGames.Client.Photon;
using iiMenu.Notifications;
using OVR;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    internal class Sound
    {
        public static void BetaPlayTag(int id, float volume)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
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
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void RandomSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    UnityEngine.Random.Range(3, 215),
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BassSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    68,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void WolfSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    195,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void CatSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    236,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void TurkeySoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    83,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void FrogSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    91,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BeeSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    191,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void EarrapeSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    215,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void DingSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    244,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BigCrystalSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    213,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void PanSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    248,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void AK47SoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    203,
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void SqueakSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    75 + (Time.frameCount % 2),
                    false,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void SirenSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    48 + ((Time.frameCount / 30) % 2) * 2,
                    false,
                    999999f
                });
                RPCProtection();
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
                GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                    soundId,
                    false,
                    999999f
                });
                RPCProtection();
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

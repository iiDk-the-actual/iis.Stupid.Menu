using Photon.Pun;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    internal class Sound
    {
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
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    1,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BrawlCountSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    6,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BrawlStartSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    7,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void TagSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    0,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void RoundEndSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    2,
                    999999f
                });
                RPCProtection();
            }
        }

        public static void BonkSoundSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlayTagSound", RpcTarget.All, new object[]
                {
                    4,
                    999999f
                });
                RPCProtection();
            }
        }
    }
}

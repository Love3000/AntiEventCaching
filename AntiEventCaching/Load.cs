using ExitGames.Client.Photon;
using Harmony;
using MelonLoader;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.SDKBase;
using VRC.Core;
using VRC;

namespace AntiEventCaching
{
    public class Load : MelonMod
    {

        public override void OnApplicationStart()
        {
            MelonLogger.Log("Had this a long time ago but the anti was able to be bypassed, only recently have people been using the exploit again, so here you go..");
            MelonLogger.Log("Also the exploit works when people spam events with long data, it gets cached, and the master sends all cached events to someone when they join, and when you send mass amounts of data super fast you lag and disconnect lol...");
            LoadPatches();
        }

        private static void LoadPatches()
        {
            new Patch(
                typeof(NetworkManager),
                typeof(Load),
                "Method_Public_Virtual_Final_New_Void_EventData_0",
                "OnEvent"
            );

            new Patch(
                typeof(PhotonNetwork),
                typeof(Load),
                "Method_Public_Static_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0",
                "OpRaiseEvent"
            );

            new Patch(
                typeof(VRC_EventDispatcherRFC),
                typeof(Load),
                "Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_0",
                "OnVRCEvent"
            );
        }

        private static bool OnEvent(EventData __0) // events received from other players
        {
            if (__0.Code == 4) // 4 is sending cached events
                return DataOKToSend(__0.CustomData);
            return true; // ik u can just do data.coed == 4 && return dataoktosend (datac.lsustomdata) but i dont like how that looks so ya :)
        }

        private static bool OpRaiseEvent(byte __0, Il2CppSystem.Object __1, Photon.Realtime.RaiseEventOptions __2, SendOptions __3) // events sent by you
        {      // event code == 4
            if (__0 == 4) // 4 is sending cached events
                return DataOKToSend(__1);
            return true;
        }

        private static bool OnVRCEvent(Player __0, VRC_EventHandler.VrcEvent __1, VRC_EventHandler.VrcBroadcastType __2, int __3, float __4) // vrcevents sent by other players (or OnEvent code 6)
        {
            try
            {
                if (__0 != null && __1 != null && !__0.IsLocalPlayer())
                {
                    if (__1.ParameterString.Length > 60 || __1.ParameterString.Contains("<color="))
                    {   // dont console log the parameterstring or u might lag when people spam 400 characters :|
                        MelonLogger.Log($"Prevented ({__0.GetName()}) from sending modified event [Length: {__1.ParameterString.Length}]");
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool DataOKToSend(Il2CppSystem.Object CPPObject)
        {
            object data = Serialization.FromIL2CPPToManaged<object>(CPPObject);
            int CachedEvents = 0;
            foreach (byte[] Array in (byte[][])data)
            {
                string ArrayToString = JsonConvert.SerializeObject(Array).Replace("\"", "");
                byte[] Bytes = Convert.FromBase64String(ArrayToString);
                if (Bytes.Length > 200) // Normal events are around 60-120, doing 200 to be safe not to block normal events.
                {
                    MelonLogger.Log($"Prevented self from sending/receiving Cached Events, bytes too high! (BYTES LENGTH: {Bytes.Length})");
                    return false;
                }
                CachedEvents++;
            }

            if (CachedEvents > 30)
            {
                MelonLogger.Log($"Prevented self from sending/receiving Cached Events, too many to send/receive at once! (COUNT: {CachedEvents})");
                return false;
            }
            return true;
        }
    }
}

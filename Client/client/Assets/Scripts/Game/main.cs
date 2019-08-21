using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gtmInterface;
using gtmEngine;
using gtmEngine.Net;
using FlatBuffers;

namespace gtmGame
{
    public class main : MonoBehaviour
    {
        private GameMgr m_gameMgr = new GameMgr();

        private LoginModel m_loginModel = new LoginModel();

        private ChatModel m_chatModel = new ChatModel();

        public string ipaddress = "192.168.0.102";

        // Start is called before the first frame update
        void Start()
        {
            m_gameMgr.DoInit();
            m_loginModel.DoInit();
            m_chatModel.DoInit();
        }

        // Update is called once per frame
        void Update()
        {
            m_gameMgr.DoUpdate();
        }

        private void OnDestroy()
        {
            m_loginModel.DoClose();
            m_gameMgr.DoClose();
            m_chatModel.DoClose();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 300, 100), "SendConnect"))
            {
                NetManager.instance.SendConnect(ipaddress, 8888);
            }

            if (GUI.Button(new Rect(0, 100, 300, 100), "SendLoginMsg"))
            {
                SendLoginMsg();
            }

            if (GUI.Button(new Rect(0, 200, 300, 100), "SendChatMsg"))
            {
                SendChatMsg();
            }

            if (GUI.Button(new Rect(0, 300, 300, 100), "SendTestMsg"))
            {
                SendTestMsg();
            }
        }

        private void SendChatMsg()
        {
            var builder = IMsgDispatcher.instance.flatBufferBuilder;
            var say = builder.CreateString("白日依山尽，黄河入海流，欲穷千里目，更上一层楼");
            fbs.ReqChat.StartReqChat(builder);
            fbs.ReqChat.AddSay(builder, say);
            var orc = fbs.ReqChat.EndReqChat(builder);
            builder.Finish(orc.Value);

            IMsgDispatcher.instance.SendFBMsg(fbs.ReqChat.HashID, builder);
        }

        private void SendLoginMsg()
        {
            var builder = IMsgDispatcher.instance.flatBufferBuilder;
            var account = builder.CreateString("春眠不觉晓，处处闻啼鸟。夜来风雨声，花落知多少。");
            var password = builder.CreateString("床前明月光，疑是地上霜，举头望明月，低头思故乡");
            fbs.ReqLogin.StartReqLogin(builder);
            fbs.ReqLogin.AddAccount(builder, account);
            fbs.ReqLogin.AddPassword(builder, password);
            var orc = fbs.ReqLogin.EndReqLogin(builder);
            builder.Finish(orc.Value);

            IMsgDispatcher.instance.SendFBMsg(fbs.ReqLogin.HashID, builder);
        }

        private void SendTestMsg()
        {
            var builder = IMsgDispatcher.instance.flatBufferBuilder;
            var account = builder.CreateString("春眠不觉晓，处处闻啼鸟。夜来风雨声，花落知多少。");
            var password = builder.CreateString("床前明月光，疑是地上霜，举头望明月，低头思故乡");
            fbs.ReqLogin.StartReqLogin(builder);
            fbs.ReqLogin.AddAccount(builder, account);
            fbs.ReqLogin.AddPassword(builder, password);
            var orc = fbs.ReqLogin.EndReqLogin(builder);
            builder.Finish(orc.Value);

            byte[] bytearray = builder.DataBuffer.ToFullArray();

            string strbyte = "";
            for (int i = 0; i < bytearray.Length; i++)
            {
                strbyte += bytearray[i].ToString() + " ";
            }
            ILogSystem.instance.Log(LogCategory.GameLogic, strbyte);

            FlatBuffers.ByteBuffer newbuf = new FlatBuffers.ByteBuffer(bytearray);

            var reqlogin = fbs.ReqLogin.GetRootAsReqLogin(builder.DataBuffer);
            ILogSystem.instance.Log(LogCategory.GameLogic, reqlogin.Account);
            ILogSystem.instance.Log(LogCategory.GameLogic, reqlogin.Password);
        }
    }
}

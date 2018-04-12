//--------------------------------------------------------------------------------
//   File      : CDWNetConnection.cs
//   author    : guoliang
//   function   : CDWNetConnection
//   date      : 2017-09-26
//   copyright : Copyright 2017 DW Inc.
//--------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;


public class CDWNetConnection : CNetConnection
{
    private struct DWNetPack : IPack
    {
        public GamePackage m_pack;
        private NetCrypte m_netCrpypte;
        public object m_sct;

        private int m_errorID;

        public int ErrorID
        {
            get
            {
                return m_errorID;
            }
            set
            {
                m_errorID = value;
            }
        }

        public bool HasData
        {
            get
            {
                return m_sct != null;
            }
        }


        public DWNetPack(NetCrypte netCrpypte)
        {
            this.m_netCrpypte = netCrpypte;
            m_pack = null;
            m_sct = null;
            m_errorID = -1;
        }

        public DWNetPack(GamePackage cst, NetCrypte netCrpypte)
        {
            this.m_netCrpypte = netCrpypte;
            this.m_pack = cst;
            m_sct = null;
            m_errorID = -1;
        }


        public byte[] PackMsg()
        {
            if (m_pack == null)
            {
                return null;
            }
            var pack = m_pack as GamePackage;
            //area id
            var areaIDBytes = BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder((short)pack.areaID));
            int areaIDLength = areaIDBytes.Length;
            //cmd
            var cmdBytes = BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder((short)pack.cmd));
            int cmdLength = cmdBytes.Length;
            //seq
            var seqBytes = BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(pack.seq));
            int seqLength = seqBytes.Length;

            //body
            int bodyLength = 0;
            if (pack.body != null)
            {
                bodyLength = pack.body.Length;
            }

            byte[] pkg = new byte[areaIDLength + cmdLength + seqLength + bodyLength];
            int offset = 0;
            System.Buffer.BlockCopy(areaIDBytes, 0,pkg, offset, areaIDLength);
            offset += areaIDLength;
            System.Buffer.BlockCopy(cmdBytes, 0, pkg, offset, cmdLength);
            offset += cmdLength;
            System.Buffer.BlockCopy(seqBytes, 0, pkg, offset, seqLength);
            offset += seqLength;
            if(bodyLength > 0)
            {
                System.Buffer.BlockCopy(pack.body, 0, pkg, offset, bodyLength);
            }

            var edata = m_netCrpypte.Encrypte(pkg);

            return edata;
        }

        public void UnPack(byte[] data, int offset, int length)
        {
            if (data != null)
            {
                var deData = m_netCrpypte.Decrypte(data, offset, length);
                var scPack = new GamePackage();

                if(deData.Length < 4)//cmd,seq
                {
                    Debug.LogError("error :svr data package length small than 8");
                    return;
                }
                //����ͷ��
                int headOffset = 0;
                //area id
                scPack.areaID = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(deData, headOffset));
                headOffset += 2;
                //cmd
                scPack.cmd = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(deData, headOffset));
                headOffset += 2;
                //seq
                scPack.seq = System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(deData, headOffset));
                headOffset += 4;

                if (deData.Length >= headOffset)
                {
                    scPack.body = new byte[deData.Length - headOffset];
                    Array.Copy(deData, headOffset, scPack.body, 0, deData.Length - headOffset);
                }
                else
                {
                    Debug.LogError("warn :svr data length is " + deData.Length + " smaller than 8");
                }
                this.m_sct = scPack;
            }
        }
    }

    public CDWNetConnection(string name) : base(name)
    {
    }

    ~CDWNetConnection()
    {

    }

    public void ConnectServer(string strAddr, string strIp, int nPort, NetToken token, ConnectCallBack ConnetCallback, ConnectRecvProtoMsg<GamePackage> connectRecvPack, GamePackage cmsg)
    {
        byte[] pkg = null;
        if (cmsg != null)
        {
            pkg = new DWNetPack(cmsg, this.m_crypte).PackMsg();
        }

        //UnityEngine.Debug.Log("connect pkg:" + System.BitConverter.ToString(pkg));
        base.ConnectServer(strAddr, strIp, nPort, token, ConnetCallback, (IPack pack) =>
        {
            //var msg = unpackMsg(pack);
            connectRecvPack((GamePackage)(((DWNetPack)pack).m_sct));
        }, pkg);
    }
    public void SendMsg(GamePackage msg)
    {
        var pack = new DWNetPack(msg, this.m_crypte);
        base.SendMsg(pack);
    }

    public override IPack GetPack()
    {
        return new DWNetPack(this.m_crypte);
    }

}

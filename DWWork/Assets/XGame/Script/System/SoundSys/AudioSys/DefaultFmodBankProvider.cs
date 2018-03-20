using UnityEngine;
using System.Collections;
using System.IO;

public class DefaultFmodBankProvider : IFmodBankProvider {

	private static string m_streamAssets;
	private static string m_persistPath;

    public static string BankExtension = ".bytes";

	public static string BankFolder = "Audio/";

	public DefaultFmodBankProvider()
	{
		m_streamAssets = Application.streamingAssetsPath;
		//if (Application.platform == RuntimePlatform.IPhonePlayer)
		//{
		//	m_persistPath = Application.temporaryCachePath;
		//}
		//else
		{
			m_persistPath = Application.persistentDataPath;
		}
	}

	#region IFmodBankProvider implementation

	public string GetAvailablePath (string bankName)
	{
		var path = GetPath (m_persistPath, bankName);
		if (File.Exists (path)) 
        {
            return path;
		}

        if (!string.IsNullOrEmpty(AreaSys.NowAreaName)) 
        {
            path = m_persistPath + "/"  + AreaSys.AreaRootName + "/" + AreaSys.NowAreaName + "/" + BankFolder + bankName + BankExtension;
            if (File.Exists(path))
            {
                return path;
            }
        }

        path = GetPath(m_streamAssets, bankName);
		return path;
	}

	#endregion

	private string GetPath(string folder, string bankName)
	{
		return folder + "/" + BankFolder + bankName + BankExtension;
	}


}

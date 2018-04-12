


public class TransferStateInputEnum<T> : TransferStateInput 
where T : struct, System.IComparable {
	T m_event;
	public TransferStateInputEnum(T eEvent){
		m_event = eEvent;
	}
	public override bool Equals(object obj) {
		if (obj is TransferStateInputEnum<T>) {
			var eEvent = obj as TransferStateInputEnum<T>;
			return m_event.Equals (eEvent.m_event);
		} else {
			return false;
		}
	}
	public override int GetHashCode() {
		return this.m_event.GetHashCode();
	}
}

public class TransferStateInputData : TransferStateInput 
{
	public IPack m_sData;
	public TransferStateInputData(IPack data) {
		m_sData = data;
	}
	
	public override bool Equals(object obj) {
		return obj is TransferStateInputData;
	}
	public override int GetHashCode() {
		return 0;
	}
}

public class TransferStateInputCmdMsg <CT, PT> : TransferStateInput 
{
	public PT m_sMsg;
	CT m_eCmd;
	public TransferStateInputCmdMsg(CT cmd, PT data) {
		m_eCmd = cmd;
		m_sMsg = data;
	}
	public TransferStateInputCmdMsg(CT cmd) 
	{
		m_eCmd = cmd;
	}
	
	public override bool Equals(object obj) {
		var objMsg = obj as TransferStateInputCmdMsg<CT, PT>;
		if (objMsg != null)
		{
			return this.m_eCmd.Equals(objMsg.m_eCmd);
		}
		return false;
	}
	public override int GetHashCode() {
		return m_eCmd.GetHashCode();
	}
}

public class TransferStateInputMsg <PT> : TransferStateInput 
{
	public PT m_sMsg;
	public TransferStateInputMsg(PT data) {
		m_sMsg = data;
	}
	public TransferStateInputMsg() {
	}
	
	public override bool Equals(object obj) {
		var objMsg = obj as TransferStateInputMsg<PT>;
		return objMsg != null;
	}
	public override int GetHashCode() {
		return 0;
	}
}


using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine<TState, TEvent> 
	where TState : IComparable 
	where TEvent : IComparable
{
	public TState CurrentStateId {
		get {
			return CurrentState.StateId;
		}
	}

	public IState CurrentStateObject
	{
		get {
			return CurrentState.AttachedState;
		}
	}

	private IInnerState<TState, TEvent> _currentState;
	private IInnerState<TState, TEvent> CurrentState { 
		get {
			return _currentState;
		}
		set
		{
			var oldState = _currentState;
			if (oldState != null)
			{
				oldState.Exit();
			}

			_currentState = value;

			if (value != null)
				value.Enter();
		}
	}
	private IInnerState<TState, TEvent> InitialState { get;set; }

	private StateDictionary<TState, TEvent> stateDict;
	private IFactory<TState, TEvent> factory;

	public StateMachine()
	{
		factory = new Factory<TState, TEvent>();
		stateDict = new StateDictionary<TState, TEvent>(factory);
	}

	private bool isRuning = false;
	private bool isInitialized = false;
	public void Initialize(TState stateId)
	{
		isInitialized = true;
		InitialState = stateDict[stateId];
	}

	public void Start()
	{
		isRuning = true;

		if (CurrentState == null)
			CurrentState = InitialState;
	}

	public void Stop()
	{
		isRuning = false;

		if (CurrentState != null)
		{
			CurrentState.Exit();
		}
	}
	
	public bool IsStarted
	{
		get { return isRuning; }
	}

	public virtual void Execute()
	{
		Check_StateMachineHasInitializedAndIsRunning();

		if (CurrentState == null)
			CurrentState = InitialState;

		CurrentState.Execute();
	}

	public void Fire(TEvent evtId, Hashtable paramters = null)
	{
		Check_StateMachineHasInitializedAndIsRunning();

		//check global trans
		if (m_globalTrans != null)
		{
			for (int i = 0; i < m_globalTrans.Count; i++)
			{
				var tr = m_globalTrans[i];
				if (tr.EventToTrigger.Equals(evtId))
				{
					CurrentState = tr.Target;
					return;
				}
			}
		}


		//then check state fire.
		var result = CurrentState.Fire(evtId, paramters);

		if (result.IsFired)
		{
			CurrentState = result.ToState;
		}

	}

	private void Check_StateMachineHasInitializedAndIsRunning()
	{
		if (!isInitialized)
		{
			throw new InvalidOperationException("Cannot execute before state machine is initialized");
		}

		if (!isRuning)
		{
			throw new InvalidOperationException("Cannot execute before state machine is running");
		}
	}


	public IInSyntax<TState, TEvent> In(TState state)
	{
		var s = stateDict[state];
		var builder = new StateBuilder<TState, TEvent>(s, stateDict, factory);

		return builder;
	}

	private IList<ITransition<TState, TEvent>> m_globalTrans;
	private IFactory<TState, TEvent> m_transFactory;
	public void AddGlobalTransition(TEvent evt, TState state)
	{
		if (m_transFactory == null)
		{
			m_transFactory = new Factory<TState, TEvent>();
		}

		var stateObj = stateDict[state];
		var tr = m_transFactory.CreateTransition(evt, stateObj);

		if (m_globalTrans == null)
		{
			m_globalTrans = new List<ITransition<TState, TEvent>>();
		}
		m_globalTrans.Add(tr);
	}

}

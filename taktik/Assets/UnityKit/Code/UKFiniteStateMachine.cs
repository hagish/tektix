using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 

*** Example ***

public enum State
{ 
	S0,
	S1,
	S2,
};

public enum Trigger
{
	T0,
	T1,
	T2,
};

FiniteStateMachine<State, Trigger> fsm = new UKFiniteStateMachine<State, Trigger> ();
		
var allways = UKFiniteStateMachine<State, Trigger>.ALWAYS;
	
int s = 0;

fsm.AddTransition(State.S0, Trigger.T0, allways, () => { s += 0; }, State.S0);
fsm.AddTransition(State.S0, Trigger.T1, allways, () => { s += 0; }, State.S1);
fsm.AddTransition(State.S0, Trigger.T2, allways, () => { s += 0; }, State.S2);

fsm.Start(State.S0);		

fsm.NotifyTrigger(Trigger.T1);

*/ 
public class UKFiniteStateMachine<TState, TTrigger> 
		where TState : struct
		where TTrigger : struct
{
		// --------------------------------------------------------
	
		// null effect - does nothing
		public static Action NOP = (() => {});
		// null guard - allways true
		public static Func<bool> ALWAYS = (() => {
				return true; });
	
		// --------------------------------------------------------
		
		private class Transition
		{
				public TState FromState { get; private set; }

				public TState ToState { get; private set; }

				public TTrigger Trigger { get; private set; }

				public Func<bool> Guard { get; private set; }

				public Action Effect { get; private set; }
			
				// trigger [guard] / effect
				public Transition (TState fromState, TTrigger trigger, Func<bool> guard, Action effect, TState toState)
				{
						this.FromState = fromState;
						this.ToState = toState;
						this.Trigger = trigger;
				
						this.Guard = guard;
						this.Effect = effect;
				}
		}
		
		// --------------------------------------------------------

		private class StateAction
		{
				public TState State { get; private set; }

				public Action Action { get; private set; }
			
				public StateAction (TState state, Action action)
				{
						this.State = state;
						this.Action = action;
				}
		}
		
		// --------------------------------------------------------
	
		public bool Initialised { get; private set; }

		public TState State { get; private set; }
		
		private LinkedList<Transition> transitions;
		private LinkedList<StateAction> stateEnterActions;
		private LinkedList<StateAction> stateLeaveActions;
	
		// --------------------------------------------------------

		public UKFiniteStateMachine ()
		{
				Initialised = false;
				transitions = new LinkedList<Transition> ();
				stateEnterActions = new LinkedList<StateAction> ();
				stateLeaveActions = new LinkedList<StateAction> ();
		}
		
		public void AddTransitions (TState[] fromStates, TTrigger trigger, Func<bool> guard, Action effect, TState toState)
		{
				foreach (TState fromState in fromStates) {
						AddTransition (fromState, trigger, guard, effect, toState);	
				}
		}

		public void AddTransition (TState fromState, TTrigger trigger, Func<bool> guard, Action effect, TState toState)
		{
				Assert (!Initialised, "already started");
			
				Transition t = new Transition (fromState, trigger, guard, effect, toState);
				transitions.AddLast (t);
		}
		
		public void AddStateEnterAction (TState state, Action enterAction)
		{
				Assert (!Initialised, "already started");
			
				StateAction sa = new StateAction (state, enterAction);
				stateEnterActions.AddLast (sa);
		}
		
		public void AddStateLeaveAction (TState state, Action leaveAction)
		{
				Assert (!Initialised, "already started");
			
				StateAction sa = new StateAction (state, leaveAction);
				stateLeaveActions.AddLast (sa);
		}
	
		private void Assert (bool assertion, string message)
		{
			if (!assertion) Debug.LogError(message);
		}

		// calls enter functions of start state
		public void Start (TState startState)
		{
				Assert (!Initialised, "already started");
			
				Initialised = true;
				State = startState;
				CallStateActionsFromList (startState, stateEnterActions);
		}
		
		// NOTE state changes to toState after effect gets executed
		public void NotifyTrigger (TTrigger trigger)
		{
				Assert (Initialised, "you need to start the statemachine first");
			
				foreach (Transition t in transitions) {
						if (t.FromState.Equals (State) && t.Trigger.Equals (trigger)) {
								if (t.Guard == null || t.Guard ()) {
										if (t.Effect != null) t.Effect ();
					
//										UnityEngine.Debug.Log(this + ": " + t.FromState + " -> " + t.ToState);
					
										SwitchState (t.ToState);
						
										return;
								}
						}
				}
		}

		// does not trigger anything, used for serialisation
		public void ForceState(TState state) {
			State = state;
		}

		// --------------------------------------------------------

		private void SwitchState (TState nextState)
		{
				if (!State.Equals (nextState)) {
						// switch needed
						CallStateActionsFromList (State, stateLeaveActions);
						State = nextState;
						CallStateActionsFromList (nextState, stateEnterActions);
				}
		}
		
		private void CallStateActionsFromList (TState state, LinkedList<StateAction> stateActions)
		{
				foreach (StateAction sa in stateActions) {
						if (sa.State.Equals (state)) {
								sa.Action ();	
						}
				}
		}
}

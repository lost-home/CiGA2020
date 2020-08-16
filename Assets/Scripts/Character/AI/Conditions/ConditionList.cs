

namespace Nez.AI.GOAP {
	
	public enum ConditionsCheckMode {
		AllTrueRequired,
		AnyTrueSuffice
	}

	public class ConditionList : AICondition {
		public ConditionsCheckMode checkMode;
		public string[] conditions;

		private AIScenarioAgent _agent;

		public override void Awake () {
			_agent = agent as AIScenarioAgent;
		}

		public override bool OnCheck () {
			if ( checkMode == ConditionsCheckMode.AnyTrueSuffice ) {
				foreach ( var condition in conditions ) {
					if ( _agent.CheckCondition ( condition ) ) {
						return true;
					}
				}

				return false;
			}
			
			foreach ( var condition in conditions ) {
				if ( _agent.CheckCondition ( condition ) ) {
					continue;
				}

				return false;
			}

			return true;
		}
	}
}
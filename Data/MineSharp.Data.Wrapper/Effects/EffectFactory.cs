using MineSharp.Core.Types;

namespace MineSharp.Data.Effects {
    public static class EffectFactory {
		public static Effect CreateEffect(Type type,
					int amplifier, DateTime startTime, int duration) {

			if (!type.IsAssignableTo(typeof(Effect)))
				throw new ArgumentException();

			object[] parameters = new object[] {
					amplifier, startTime, duration
				};

			return (Effect)Activator.CreateInstance(type, parameters)!;
		}

		public static Effect CreateEffect(int id,
						int amplifier, DateTime startTime, int duration) {
			var type = EffectPalette.GetEffectTypeById(id);
			return CreateEffect(type, amplifier, startTime, duration);
		}
	}
}

using MineSharp.Core.Types;

namespace MineSharp.Data.Entities {
    public class EntityFactory {

		public static Entity CreateEntity(Type type,
				int serverId, Vector3 position,
				float pitch, float yaw,
				Vector3 velocity, bool isOnGround,
				Dictionary<int, Effect?> effects) {

			if (!type.IsAssignableTo(typeof(Entity)))
				throw new ArgumentException();

			object[] parameters = new object[] {
						serverId,
						position,
						pitch, yaw,
						velocity,
						isOnGround,
						effects
				};

			return (Entity)Activator.CreateInstance(type, parameters)!;
		}

		public static Entity CreateEntity(int id,
						int serverId, Vector3 position,
						float pitch, float yaw,
						Vector3 velocity, bool isOnGround,
						Dictionary<int, Effect?> effects) {
			var type = EntityPalette.GetEntityTypeById(id);
			return CreateEntity(type, serverId, position, pitch, yaw, velocity, isOnGround, effects);
		}

	}
}

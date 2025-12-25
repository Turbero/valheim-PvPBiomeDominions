using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PvPBiomeDominions.Helpers
{
    public static class Guilds_API_Reflection
    {
        private static Type _apiType;

        private static MethodInfo _isLoadedMethod;
        private static MethodInfo _getPlayerGuildMethod;
        private static MethodInfo _getGuildIconByIdMethod;
        private static Type getGuildsAPIType()
        {
            if (_apiType == null)
            {
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(asm => asm.GetName().Name.Equals("Guilds", StringComparison.OrdinalIgnoreCase));
                if (asm != null)
                    _apiType = asm.GetType("Guilds.API");
            }

            return _apiType;
        }

        public static bool IsLoaded()
        {
            if (_isLoadedMethod == null)
                _isLoadedMethod = getGuildsAPIType().GetMethod(
                    "IsLoaded",
                    BindingFlags.Public | BindingFlags.Static);

            try
            {
                return (bool)_isLoadedMethod.Invoke(null, null);
            }
            catch
            {
                return false;
            }
        }

        public static object GetPlayerGuild(Player player)
        {
            if (_getPlayerGuildMethod == null)
            {
                _getPlayerGuildMethod = getGuildsAPIType().GetMethod(
                    "GetPlayerGuild",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[]{ typeof(Player)},
                    null);
            }

            try
            {
                return _getPlayerGuildMethod.Invoke(null, new object[] { player });
            }
            catch
            {
                return null;
            }
        }

        public static Sprite GetGuildIconById(int iconId)
        {
            if (_getGuildIconByIdMethod == null)
            {
                _getGuildIconByIdMethod = getGuildsAPIType().GetMethod(
                    "GetGuildIconById",
                    BindingFlags.Public | BindingFlags.Static);
            }

            try
            {
                return _getGuildIconByIdMethod.Invoke(null, new object[] { iconId }) as Sprite;
            }
            catch
            {
                return null;
            }
        }

        // ----------------------------------------------------
        // Reflection helpers
        // ----------------------------------------------------
        public static T GetGuildProperty<T>(object obj, string propertyName)
        {
            if (obj == null)
                return default;

            try
            {
                var prop = obj.GetType().GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.Instance);

                if (prop == null)
                    return default;

                object value = prop.GetValue(obj);
                return value is T typed ? typed : default;
            }
            catch
            {
                return default;
            }
        }

        public static T GetNestedProperty<T>(object obj, params string[] path)
        {
            object current = obj;

            foreach (string name in path)
            {
                if (current == null)
                    return default;

                var prop = current.GetType().GetProperty(
                    name,
                    BindingFlags.Public | BindingFlags.Instance);

                if (prop == null)
                    return default;

                current = prop.GetValue(current);
            }

            return current is T value ? value : default;
        }
    }
}

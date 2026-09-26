using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

public class UserRoleConverter : JsonConverter<UserRole>
{

	internal static readonly string NAME_PROPERTY = "Name";
	internal static readonly string PERMISSIONS_PROPERTY = "Permissions";

	public override UserRole? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		using JsonDocument document = JsonDocument.ParseValue(ref reader);

		List<string> propertyList = [.. document.RootElement.EnumerateObject().Select(element => element.Name)];

		UserRole role = new()
		{
			Name = QuickReadString(document, propertyList, NAME_PROPERTY)
		};

		propertyList.Remove(PERMISSIONS_PROPERTY);
		role.permissionSettings = ReadPermissions(document);

		if (propertyList.Count > 0)
			throw new JsonException($"The {nameof(UserRole)} json had left-over properties after parsing, such as {propertyList[0]}. These unused properties cannot be accounted for, and may result in data loss. Is it from a newer version?");

		return role;

	}

	public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
	{

		writer.WriteStartObject();

		// Write name
		writer.WriteString(NAME_PROPERTY, value.Name);

		// Write permissions
		writer.WriteStartObject(PERMISSIONS_PROPERTY);
		foreach (UserPermission permission in UserPermission.ALL)
			QuickWritePermission(writer, permission, value);
		writer.WriteEndObject();

		writer.WriteEndObject();

	}

	private static void QuickWritePermission(Utf8JsonWriter writer, UserPermission permission, UserRole value)
	{

		bool? result = value.GetPermission(permission);

		if (result is bool boolResult)
			writer.WriteBoolean(permission.DataName, boolResult);
		else
			writer.WriteNull(permission.DataName);

	}

	private static string QuickReadString(JsonDocument document, List<string> propertyList, string name)
	{

		propertyList.Remove(name);

		return document.RootElement.GetProperty(name).GetString()
			?? throw new JsonException($"{name} should not be null in {document}");

	}

	private static Dictionary<UserPermission, bool> ReadPermissions(JsonDocument document)
	{

		Dictionary<UserPermission, bool> permissionSettings = [];
		JsonElement jsonObj = document.RootElement.GetProperty(PERMISSIONS_PROPERTY);

		foreach (JsonProperty property in jsonObj.EnumerateObject())
		{

			string dataName = property.Name;

			// We get this first, because if we can't find a permission even for a "null" permission, there's a chance we could be causing data loss in a different role.
			UserPermission permission = UserPermission.ALL.First(perm => perm.DataName == dataName);

			bool? value = property.Value.ValueKind switch
			{
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Null => null,
				_ => throw new JsonException($"Unsupported value kind for {dataName}: {property.Value.ValueKind}")
			};

			if (value is bool boolValue)
				permissionSettings.Add(permission, boolValue);

		}

		return permissionSettings;

	}

}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

public class UserRoleConverter : JsonConverter<UserRole>
{

	/// <summary>
	/// The property that the role's name will be stored in.
	/// </summary>
	internal static readonly string NAME_PROPERTY = "Name";

	/// <summary>
	/// The property that the role's permissions will be stored in.
	/// </summary>
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

	/// <summary>
	/// Write true/false/null for <paramref name="value"/>'s <paramref name="permission"/> to <paramref name="writer"/>.
	/// </summary>
	/// <param name="writer">The <see cref="Utf8JsonWriter"/> to write this boolean/null to; expecting to be in an open object.</param>
	/// <param name="permission">The <see cref="UserPermission"/> to write about.</param>
	/// <param name="value">The <see cref="UserRole"/> coming from <see cref="Write"/>.</param>
	private static void QuickWritePermission(Utf8JsonWriter writer, UserPermission permission, UserRole value)
	{

		bool? result = value.GetPermission(permission);

		if (result is bool boolResult)
			writer.WriteBoolean(permission.DataName, boolResult);
		else
			writer.WriteNull(permission.DataName);

	}

	/// <summary>
	/// Quickly read <paramref name="name"/> (string) from <paramref name="document"/> and remove <paramref name="name"/> from <paramref name="propertyList"/>.
	/// </summary>
	/// <param name="document">The <see cref="JsonDocument"/> to read from.</param>
	/// <param name="propertyList">The <see cref="List{string}"/> to remove <paramref name="name"/> from.</param>
	/// <param name="name">The <see cref="string"/> to read from <see cref="JsonDocument"/> and to remove from <paramref name="propertyList"/>.</param>
	/// <returns>Returns the read string.</returns>
	/// <exception cref="JsonException">If name is null.</exception>
	private static string QuickReadString(JsonDocument document, List<string> propertyList, string name)
	{

		propertyList.Remove(name);

		return document.RootElement.GetProperty(name).GetString()
			?? throw new JsonException($"{name} should not be null in {document}");

	}

	/// <summary>
	/// Read the permissions from <paramref name="document"/> and turn them into a <see cref="Dictionary{UserPermission, bool}"/>.
	/// </summary>
	/// <param name="document">The <see cref="JsonDocument"/> to read <see cref="PERMISSIONS_PROPERTY"/> from.</param>
	/// <returns>A <see cref="Dictionary{UserPermission, bool}"/> containing modified permissions.</returns>
	/// <exception cref="JsonException">If a value wasn't true, false, or null.</exception>
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

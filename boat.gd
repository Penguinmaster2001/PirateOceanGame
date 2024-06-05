extends Node3D

# @export_range (0.0, 300.0) var travel_speed := 25.0
var travel_speed := 100.0

var cur_hex: Hex
var next_hex: Hex
var target_pos := Vector3.ZERO

# Array of the hexes the boat is following
var travel_list := [ ]

var click_marker : MeshInstance3D
var boat_model : Node3D


var keys_to_dirs := {
	KEY_W: 0, # Right
	KEY_Q: 1, # Up Right
	KEY_A: 2, # Up Left
	KEY_S: 3, # Left
	KEY_D: 4, # Down Left
	KEY_E: 5  # Down Right
}



func _ready() -> void:
	click_marker = get_parent().get_child(2)
	boat_model = get_child(1)



func _process(delta: float) -> void:
	var dist_to_target := position.distance_to(target_pos)
	var dir_to_target := position.direction_to(target_pos)

	if dist_to_target > 5.0:
		position += (travel_speed * clampf(dist_to_target / 10.0, 0.25, 3.0) * delta) * dir_to_target

	if dist_to_target < 25.0:
		cur_hex = next_hex
		if travel_list.is_empty():
			return
		
		next_hex = travel_list.pop_front()
		target_pos = next_hex.get_world_coords(25.0)
		boat_model.call("update_look_at_target", target_pos)



func _change_tile(direction: int) -> void:
	var dir := cur_hex.get_neighbor_coord(direction)

	var new_hex := HexMap.get_hex(dir._q, dir._r)

	print(new_hex as WfcHex)

	if new_hex != null:
		cur_hex = new_hex
		target_pos = cur_hex.get_world_coords(25)



func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		var camera := get_viewport().get_camera_3d()

		var origin := camera.project_ray_origin(event.position)
		var direction := camera.project_ray_normal(event.position)

		var intersection := origin - ((origin.y / direction.y) * direction)

		click_marker.position = intersection

		var target_hex := HexMap.get_hex_at_world_coords(intersection.x, intersection.z)
		# travel_list.append_array(HexMap.get_hexes_on_line(cur_hex, target_hex))
		travel_list.append_array(HexMap.find_path(cur_hex, target_hex))

		# print("~~~~~")
		# print(intersection)
		# print("Current: ", cur_hex)
		# print("Target: ", target_hex)
		# print(travel_list)

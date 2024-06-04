extends Node3D

# @export_range (0.0, 300.0) var travel_speed := 25.0
var travel_speed := 300.0

var cur_hex: Hex
var target_pos := Vector3.ZERO


var keys_to_dirs := {
	KEY_W: 0, # Right
	KEY_Q: 1, # Up Right
	KEY_A: 2, # Up Left
	KEY_S: 3, # Left
	KEY_D: 4, # Down Left
	KEY_E: 5  # Down Right
}



func _ready() -> void:
	cur_hex = HexMap.get_hexes()[0]



func _process(delta: float) -> void:
	var dist_to_target := position.distance_to(target_pos)
	var dir_to_target := position.direction_to(target_pos)

	if dist_to_target > 1.0:
		position += (travel_speed * clampf(dist_to_target / 25.0, 0.25, 3.0) * delta) * dir_to_target



func _change_tile(direction: int) -> void:
	var dir := cur_hex.get_neighbor_coord(direction)

	var new_hex := HexMap.get_hex(dir._q, dir._r)

	print(new_hex as WfcHex)

	if new_hex != null:
		cur_hex = new_hex
		target_pos = cur_hex.get_world_coords(25)



func _input(event: InputEvent) -> void:
	if not event is InputEventKey or event.is_echo() or event.is_pressed():
		return
	
	if event.get_keycode() in keys_to_dirs:
		_change_tile(keys_to_dirs[event.get_keycode()])

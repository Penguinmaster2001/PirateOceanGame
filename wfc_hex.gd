extends Hex

class_name WfcHex

var collapsed := false

# var valids := [Hex.WATER, Hex.SHORE, Hex.FOREST, Hex.MOUNT, Hex.PORT]
# var valids := [[0, 6], [0, 6], [0, 6], [0, 6], [0, 6], [0, 6]]
var valids := [6, 6, 6, 6, 6, 6]
var num_options := 6



func _init(init_q: int, init_r: int) -> void:
	super(init_q, init_r)



func get_num_options() -> int:
	return num_options



func set_valids(new_valids: Array) -> void:
	valids = new_valids



func collapse(type: int) -> void:
	collapsed = true

	terrain_type = type



func is_collapsed() -> bool:
	return collapsed



func _to_string() -> String:
	return "\n---\nhex(q: %d, r: %d)\n%s\n%s\n" % [self.q, self.r, self.collapsed, self.valids]

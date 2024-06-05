
extends Node3D

const TILE_SIZE := 25.0
const SIDE_RATIO := cos(PI / 6.0)
const HEX_TILE := preload("res://hex_tile.tscn")

@export_range (1, 300) var grid_size := 10


var hex_textures := [ ]



func _ready() -> void:
	var texture_paths := HexMap.get_hex_types("res://hex_type_data.json")

	for path: String in texture_paths:
		hex_textures.append(load(path))

	HexMap.generate_triangle(grid_size);

	# HexMap.collapse_next_hex()

	_display_map(HexMap.get_collapsed_hexes())



func _process(_delta: float) -> void:
	if HexMap.collapsed_all_hexes():
		return

	for i in range(60):
		_display_hex(HexMap.collapse_next_hex())

	# HexMap.display_data()



func _display_map(hexes: Array) -> void:
	for hex: Hex in hexes:
		_display_hex(hex)
	


func _display_hex(hex: Hex) -> void:
	if hex == null:
		return

	var tile_coords := hex.get_world_coords(TILE_SIZE)

	var tile := HEX_TILE.instantiate()
	add_child(tile)
	tile.translate(tile_coords)
	
	tile.get_child(0).material_override = hex_textures[hex.get_terrain_type()]


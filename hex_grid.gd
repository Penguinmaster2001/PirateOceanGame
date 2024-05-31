
extends Node3D

const TILE_SIZE := 25.0
const SIDE_RATIO := cos(PI / 6.0)
const HEX_TILE := preload("res://hex_tile.tscn")

@export_range (1, 500) var grid_size := 10


var hex_textures := {
	Hex.UNDEF:	preload("res://TileTextures/undefined.tres"),
	Hex.WATER:	preload("res://TileTextures/water.tres"),
	Hex.SHORE:	preload("res://TileTextures/shore.tres"),
	Hex.FOREST:	preload("res://TileTextures/forest.tres"),
	Hex.MOUNT:	preload("res://TileTextures/mountain.tres"),
	Hex.PORT:	preload("res://TileTextures/port.tres")
}



func _ready() -> void:
	HexMap.generate_triangle(grid_size);

	HexMap.generate_terrain_types();

	_display_map(HexMap.get_hexes())



func _display_map(hexes: Array) -> void:
	for hex: Hex in hexes:
		var tile_coords := hex.get_world_coords(TILE_SIZE)

		var tile := HEX_TILE.instantiate()
		add_child(tile)
		tile.translate(tile_coords)
		
		tile.get_child(0).material_override = hex_textures[hex.get_terrain_type()]

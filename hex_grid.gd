
extends Node3D

const TILE_SIZE := 25.0
const SIDE_RATIO := cos(PI / 6.0)
const HEX_TILE = preload("res://hex_tile.tscn")

@export_range (1, 100) var grid_size := 10

var updated = true



func _ready():
	HexMap.generate_triangle(grid_size);

	_display_map(HexMap.get_hexes())



func _display_map(hexes):
	for hex in hexes:
		var tile_coords = hex.get_world_coords(TILE_SIZE)

		var tile = HEX_TILE.instantiate()
		add_child(tile)
		tile.translate(tile_coords)

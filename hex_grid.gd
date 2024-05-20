
extends Node3D

const TILE_SIZE := 1.0
const SIDE_RATIO := cos(PI / 6.0)
const HEX_TILE = preload("res://hex_tile.tscn")

@export_range (1, 100) var grid_size := 10

var updated = true

func _ready():
	var map = HexMap.new(grid_size);

	_display_map(map)



func _display_map(map: HexMap):
	for hex in map.get_hexes():
		var tile_coords = hex.get_world_coords(TILE_SIZE)

		var tile = HEX_TILE.instantiate()
		add_child(tile)
		tile.translate(tile_coords)
		



func _generate_grid():
	for x in range(grid_size):
		var tile_coords := Vector3.ZERO

		tile_coords.x = x * TILE_SIZE * SIDE_RATIO
		tile_coords.z = x * TILE_SIZE / 2.0

		for y in range(grid_size):
			var tile = HEX_TILE.instantiate()
			add_child(tile)
			tile.translate(tile_coords)

			tile_coords.z += TILE_SIZE

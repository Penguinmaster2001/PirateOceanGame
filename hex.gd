
extends Node

class_name Hex

# This contains simple operations on the hexagon grid

# Right now this only really does stuff with Vector3i
# But it should be able to take in hex_tiles for some operations eventually

static var directions := [
	Hex.new( 1,  0), # Right
	Hex.new( 1, -1), # Up Right
	Hex.new( 0, -1), # Up Left
	Hex.new(-1,  0), # Left
	Hex.new(-1,  1), # Down Left
	Hex.new( 0,  1)  # Down Right
]



# Cube coords
var q: int
var r: int
var s: int

# This class will eventually need to include other stuff such as
# Terrain type
# model
# other stuff idk

# The terrain type determines the wave function collapse stuff

enum {
	UNDEF,
	WATER,
	SHORE,
	FOREST,
	MOUNT,
	PORT
}

var terrain_type : int = UNDEF



func _init(init_q: int, init_r: int) -> void:
	self.q = init_q
	self.r = init_r
	self.s = -init_q - init_r



func get_cube_coords() -> Vector3i:
	return Vector3i(q, r, s)

func get_axial_coords() -> Vector2i:
	return Vector2i(q, r)

func get_world_coords(size: float) -> Vector3:
	# Pointy
	var x := size * ((sqrt(3.0) * self.q)  +  ((sqrt(3.0) / 2.0) * self.r))
	var y := size * 1.5 * self.r
	return Vector3(x, 0, y)


func get_mag() -> int:
	return int((abs(self.q) + abs(self.r) + abs(self.s)) / 2)

func get_distance(other: Hex) -> int:
	return self.sub(other).get_mag()



func add(other: Hex) -> Hex:
	return Hex.new(self.q + other.q, self.r + other.r)

func sub(other: Hex) -> Hex:
	return Hex.new(self.q - other.q, self.r - other.r)

func scale(f: int) -> Hex:
	return Hex.new(self.q * f, self.r * f)


func get_neighbor_coord(direction: int) -> Hex:
	return self.add(directions[direction % 6])



func get_terrain_type() -> int:
	return terrain_type



func eq(other: Hex) -> bool:
	return self.q == other.q && self.r == other.r



func _to_string() -> String:
	return "Hex(q: %d, r: %d, s: %d)" % [self.q, self.r, self.s]


extends Node3D

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
var _q: int
var _r: int
var _s: int

# This class will eventually need to include other stuff such as
# Terrain type
# model
# other stuff idk

var terrain_type : int = 0



func _init(init_q: int, init_r: int) -> void:
	self._q = init_q
	self._r = init_r
	self._s = -init_q - init_r



func get_world_coords(size: float) -> Vector3:
	# Pointy
	var x := size * ((sqrt(3.0) * self._q)  +  ((sqrt(3.0) / 2.0) * self._r))
	var y := size * 1.5 * self._r
	return Vector3(x, 0, y)


func get_mag() -> int:
	return int((abs(self._q) + abs(self._r) + abs(self._s)) / 2)

func get_distance(other: Hex) -> int:
	return self.sub(other).get_mag()



func add(other: Hex) -> Hex:
	return Hex.new(self._q + other._q, self._r + other._r)

func sub(other: Hex) -> Hex:
	return Hex.new(self._q - other._q, self._r - other._r)

func scale(f: int) -> Hex:
	return Hex.new(self._q * f, self._r * f)


func get_neighbor_coord(direction: int) -> Hex:
	return self.add(directions[direction % 6])



func get_terrain_type() -> int:
	return terrain_type



func eq(other: Hex) -> bool:
	return self._q == other._q && self._r == other._r



func _to_string() -> String:
	return "Hex(_q: %d, _r: %d, _s: %d)" % [self._q, self._r, self._s]

extends Node

class_name HexShortlist



# A dictionary of arrays
# Keys are number of current_options
# Values are an array of nodes with that number of current_options
var shortlist := { }

# This is a list of all the current_options that actually contain hexes
var current_options := [ ]

# The number of hexes in the shortlist
var num_hexes := 0



# If the hex is already in the shortlist, move it to it's new number of options
# Otherwise just add it
func update_or_insert(hex: WfcHex, previous_num: int) -> void:
	# Remove the hex from previous spot
	if shortlist.get(previous_num, null) != null and shortlist[previous_num].has(hex):
		shortlist[previous_num].erase(hex)
		num_hexes -= 1

		if shortlist[previous_num].is_empty():
			current_options.erase(previous_num)
	
	# Insert into new spot
	insert(hex)



# Put the hex into the shortlist
# Add a new list if needed, update least_options if necessary
func insert(hex: WfcHex) -> void:
	var num_options := hex.get_num_options()

	if shortlist.get(num_options, null) == null:
		shortlist[num_options] = [ ]

		_add_option(num_options)
	
	if not shortlist[num_options].has(hex):
		shortlist[num_options].append(hex)
		num_hexes += 1



# Remove a hex from the shortlist
# Update least_options if necessary
func remove(hex: WfcHex) -> void:
	var num_options := hex.get_num_options()

	if shortlist.get(num_options, null) == null:
		return
	
	if not shortlist[num_options].has(hex):
		return
	
	shortlist[num_options].erase(hex)
	num_hexes -= 1

	if shortlist[num_options].is_empty():
		current_options.erase(num_options)



# Get a random hex from the least constrained
func get_most_constrained_random() -> WfcHex:
	if current_options.is_empty():
		return null
	
	return shortlist[current_options[0]].pick_random()



# Add an option to the current_options, keeping it in order
func _add_option(option: int) -> void:
	if current_options.has(option):
		return

	var index := current_options.bsearch(option)

	current_options.insert(index, option)


		
func count() -> int:
	return num_hexes



func is_empty() -> bool:
	return num_hexes == 0

class_name MediaList
extends ScrollContainer

@export var type: Session.Type

@onready var v_box_container: VBoxContainer = $VBoxContainer

var sessions: Dictionary[String, MediaDisplay] = {}

signal switch(aumid: String, type: Session.Type)

func has_aumid(aumid: String) -> bool:
	return sessions.has(aumid)

func new_display(aumid: String, media_display: MediaDisplay) -> void:
	v_box_container.add_child(media_display)
	media_display.button.pressed.connect(
		func(): switch.emit(aumid, type),
		CONNECT_ONE_SHOT # Only fire once since when its fired, it will need to change parent
	)
	sessions.set(aumid, media_display)

func remove_display(aumid: String) -> MediaDisplay:
	var media_display: MediaDisplay = sessions.get(aumid)
	if media_display != null:
		sessions.erase(aumid)
		v_box_container.remove_child(media_display)
	
	return media_display

func reset_list() -> void:
	for child in v_box_container.get_children(true):
		if child is MediaDisplay:
			v_box_container.remove_child(child)
			child.queue_free()
	sessions = {}

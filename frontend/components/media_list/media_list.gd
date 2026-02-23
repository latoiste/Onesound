class_name MediaList
extends ScrollContainer

@onready var media_container: VBoxContainer = $VBoxContainer

func add_new_media(aumid: String, icon: Texture2D) -> void:
	var media_display := MediaDisplay.new_media_display(aumid, icon)
	media_container.add_child(media_display)

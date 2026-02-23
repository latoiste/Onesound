class_name MediaDisplay
extends Control

@export var icon_texture: Texture2D
@export var aumid_text: String
@onready var app_icon: TextureRect = $HSplitContainer/AppIcon
@onready var aumid: Label = $HSplitContainer/VBoxContainer/Aumid

const MEDIA_DISPLAY_SCENE: PackedScene = preload("uid://bgipxrwuktnps")
const HEIGHT_RATIO := 0.15

static func new_media_display(_aumid_text: String, _icon_texture: Texture2D) -> MediaDisplay:
	var media_display: MediaDisplay = MEDIA_DISPLAY_SCENE.instantiate()
	media_display.aumid_text = _aumid_text
	media_display.icon_texture = _icon_texture
	
	return media_display

func _ready() -> void:
	var viewport_height = get_viewport().size.y
	custom_minimum_size.y = viewport_height * HEIGHT_RATIO
	
	#TODO: add a default icon_texture
	if icon_texture != null:
		app_icon.texture = icon_texture
	aumid.text = aumid_text

class_name MediaDisplay
extends Control

@onready var app_icon: TextureRect = $HSplitContainer/AppIcon
@onready var aumid_label: Label = $HSplitContainer/HSplitContainer/Aumid
@onready var button: Button = $HSplitContainer/HSplitContainer/Button

var icon_texture: Texture2D
var aumid: String
var type: Session.Type

const ADD_ICON: Texture2D = preload("uid://r42eypsp34ua")
const REMOVE_ICON: Texture2D = preload("uid://bff8ca0srkf1i")
const DEFAULT_ICON: Texture2D = preload("uid://daldc0d5vuqui")
const MEDIA_DISPLAY_SCENE: PackedScene = preload("uid://bgipxrwuktnps")
const HEIGHT_RATIO := 0.15

static func new_media_display(_aumid: String, _icon_texture: Texture2D, _type: Session.Type) -> MediaDisplay:
	var media_display: MediaDisplay = MEDIA_DISPLAY_SCENE.instantiate()
	media_display.aumid = _aumid
	media_display.icon_texture = _icon_texture
	media_display.type = _type
	
	return media_display

func _ready() -> void:
	var viewport_height = get_viewport().size.y
	custom_minimum_size.y = viewport_height * HEIGHT_RATIO
	
	if icon_texture != null:
		app_icon.texture = icon_texture
	else:
		app_icon.texture = DEFAULT_ICON
	aumid_label.text = aumid
	
	button.icon = ADD_ICON if type == Session.Type.AVAILABLE else REMOVE_ICON
	button.pressed.connect(_switch_icon)

func _switch_icon():
	if button.icon == ADD_ICON:
		button.icon = REMOVE_ICON
	else:
		button.icon = ADD_ICON

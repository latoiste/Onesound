class_name HttpHandler
extends Node

@onready var awaitable_http_request: AwaitableHTTPRequest = $AwaitableHTTPRequest

func _ready() -> void:
	awaitable_http_request.timeout = 1.0
	
## Returns an [Array] of sessions represented as [Dictionary]. 
## The dictionary will have the keys:[br]
## - aumid: String [br]
## - displayName: String [br]
## - base64EncodedIcon: String [br]
func request_media_get(registered: bool = false) -> Array:
	var body: Array
	var resp := await awaitable_http_request.async_request("http://127.0.0.1:5000/media?registered=%s" % registered)
	if resp.status_ok() && resp.success():
		body = resp.body_as_json()
	return body

func request_media_post(aumid: String) -> bool:
	var resp = await awaitable_http_request.async_request(
		"http://127.0.0.1:5000/media/%s" % aumid, 
		PackedStringArray(), 
		HTTPClient.Method.METHOD_POST,
	)
	
	if resp.status_ok() && resp.success():
		return true
	return false
	
func request_media_delete(aumid: String) -> bool:
	var resp = await awaitable_http_request.async_request(
		"http://127.0.0.1:5000/media/%s" % aumid,
		PackedStringArray(), 
		HTTPClient.Method.METHOD_DELETE,
	)
	
	if resp.status_ok() && resp.success():
		return true
	return false

func check_backend_health() -> bool:
	var resp = await awaitable_http_request.async_request("http://127.0.0.1:5000/health")

	if resp.status_ok() && resp.success():
		return true
	return false

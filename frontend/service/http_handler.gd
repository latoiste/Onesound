class_name HttpHandler
extends Node

@onready var awaitable_http_request: AwaitableHTTPRequest = $AwaitableHTTPRequest

func _ready() -> void:
	awaitable_http_request.timeout = 5.0
	
# This will return an Array[String]
func request_media_get() -> Array:
	var body: Array
	var resp := await awaitable_http_request.async_request("http://127.0.0.1:5000/media")
	
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

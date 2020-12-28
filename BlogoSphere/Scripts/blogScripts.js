function addTag(tagname, id) {
	if (tagname == "") { tagname = $("#tagName").val(); }
	$.ajax({
		url: "/Post/AttachTag",
		data: { name: tagname },
		success: function (data) {
			$("#notags").hide();
			if (id != null) { $("#tag_" + id).hide(); }
			$("#moretags").html(""); // Clear the div
			$.each(data, function (idx, tag) {
				$("#moretags").append('<div class="rounded-pill pr-1 pl-3 pt-1 pb-1 mr-2 mb-2" style="background-color: #e59560" >' + tag['Name'] + ' <a href="javascript:;" onclick="subTag(\'' + tag['Name'] + '\')" class="text-danger p-0 pr-1"><i class="far fa-times-circle"></i></a></div>')
			});
			$("#moretags").append('<div><input type="text" id="tagName" value="" placeholder="Add tag..." class="rounded-pill form-control-sm border-0" autocomplete="off" />' +
				'<input type="button" value="Add" onclick="addTag(\'\', null)" class="btn-sm text-light border-0" style="background-color: #1d3124" /></div>');
		}
	});
}

function subTag(tagname) {
	$.ajax({
		url: "/Post/DetachTag",
		data: { name: tagname },
		success: function (data) {
			$("#moretags").html(""); // Clear the div
			$.each(data, function (idx, tag) {
				$("#moretags").append('<div class="rounded-pill pr-1 pl-3 pt-1 pb-1 mr-2 mb-2" style="background-color: #e59560">' + tag['Name'] + ' <a href="javascript:;" onclick="subTag(\'' + tag['Name'] + '\')" class="text-danger p-0 pr-1"><i class="far fa-times-circle"></i></a></div>')
			});
			$("#moretags").append('<div><input type="text" id="tagName" value="" placeholder="Add tag..." class="rounded-pill form-control-sm border-0" autocomplete="off" />' +
				'<input type="button" value="Add" onclick="addTag(\'\', null)" class="btn-sm text-light border-0" style="background-color: #1d3124" /></div>');
		}
	});
}

function countChar(val) {
	var len = val.value.length;
	if (len >= 1024) {
		val.value = val.value(0, 1024);
	}
	else {
		$('#charCount').text(1024 - len);
	}
}
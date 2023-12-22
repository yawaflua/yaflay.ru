function updatePreview(value) {
    var blogText = $("#previewCard").children("#code").children("#blogText");
    blogText.empty();
    blogText.append(value);
}
function updateAnnotation(value) {
    var blogText = $("#annotationCard").children("#code").children("#annotationText");
    blogText.empty();
    blogText.append(value);
}

function sendArticleData() {
    let melon = document.cookie.split(';')[0].replace("melon=", "");
    let body = `{
    "title": "${$("#titleInput").val().replace(/"/g, '\"')}",
    "annotation": "${$("#annotationArea").val().replace(/\n/g, " \\n").replace(/"/g, '\"')}}",
    "text": "${$("#textArea").val().replace(/\n/g, " \\n").replace(/"/g, '\"')}",
    "image": "${$("#imgInput").val()}",
    "author": "${melon}"
}`;
    fetch("/api/Blog", {
        method: "POST",
        headers: {
            Authorization: `Bearer ${melon}`,
            "Content-Type": "application/json"
        },
        body: body
    }).then(k => console.log(k));
    $("#titleInput").val('');
    $("#annotationArea").val('');
    $("#textArea").val('');
    $("#imgInput").val('');
}


function sendRedirectData() {
    console.log("click!")
    let melon = document.cookie.split(';')[0].replace("melon=", "");
    let body = `{
        "url": "${$("#urlInput").val()}",
        "uri": "${$("#uriInput").val()}",
        "author": "${melon}"
    }`;
    fetch("/api/redirects", {
        method: "POST",
        headers: {
            Authorization: `Bearer ${melon}`,
            "Content-Type": "application/json"
        },
        body: body
    }).then(k => console.log(k));
    $("#urlInput").val('');
    $("#uriInput").val('');
}


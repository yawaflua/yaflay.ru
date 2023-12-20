let blogId = $("#blogId").text();
function loadComments() {
    if (document.location.pathname.startsWith("/Blog")) {
        fetch(`/api/Blog/${blogId}/comments`)
            .then(response => {
                let data = response.json();
                data.then(k => {
                    for (let i = 0; i < k.length; i++) {
                        let date = new Date(k[i].dateTime * 1000);
                        $("#commentBar").after(
                            `<div class="d-flex flex flex-start bg-dark bot-1">
                            <div class="container">
                                <h6 class="fw-bold mb-1">${k[i].creatorMail}</h6>
                                <div class="d-flex align-items-center mb-3">
                                    <p class="mb-0">
                                        ${date.toLocaleString()}
                                    </p>
                                </div>
                                <p class="mb-0">
                                    ${k[i].text}
                                </p>
                            </div>
                        </div>`
                        )
                    }
                });
            });
    }
}
$("#postComment").click(
    function () {
        var contentBody = {
            text: $("#commentText").val(),
            sender: $("#userEmail").val()
        }
        $.ajax(`/api/Blog/${blogId}/comments`, {
            data: JSON.stringify(contentBody),
            contentType: "application/json",
            method: "post"
        }).done(response => { $("#commentText").val(''); $("#userEmail").val(''); $("#commentBar").empty(); loadComments();  })

        }
        );
$(loadComments());
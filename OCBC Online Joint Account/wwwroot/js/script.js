const faqs = document.querySelectorAll(".faq");

faqs.forEach(faq => {
    faq.addEventListener("click", () => {
        faq.classList.toggle("active");
    });
});

$("div").on("click", "#ButtonDisable_Click", function () {
    $("#ButtonDisabled").modal("show");
    setTimeout(function () {
        $("#ButtonDisabled").modal("hide");
    }, 1000);
});
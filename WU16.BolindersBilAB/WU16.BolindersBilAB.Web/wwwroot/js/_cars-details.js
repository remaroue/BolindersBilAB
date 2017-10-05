
(function () {
    const overlayTemplate = "<div class='card-modal-overlay'></div>";
    const modalTemplate = "<div class='card card-modal'></div>";
    const $modalClass = ".card-modal";
    const $overlayClass = ".card-modal-overlay";

    function open(content, customClasses) {
        $("body").append(overlayTemplate);
        $("body").append(modalTemplate);

        console.log($($modalClass)
            .addClass(customClasses));

        $($modalClass)
            .append(content);

        $($overlayClass).on("click", close);

        return $($modalClass);
    }

    function changeContent(newContent) {
        $($modalClass).empty().append(newContent);
    }

    function close() {
        $($modalClass).remove();
        $($overlayClass).remove();
    }

    window.CustomModal = {
        open: open,
        close: close,
        changeContent: changeContent
    };
})();

$(document).ready(function () {
    if ($(".no-images").length === 0) {
        const currentImg = "#cars-details-container .current-image img";

        $("#cars-details-container .image-preview img").on("click", function (e) {
            $(currentImg)
                .off("click")
                .replaceWith(e.target.cloneNode());

            addClickEvent();
            findHeight();
        });
        function addClickEvent() {
            $(currentImg).on("click", function (e) {
                CustomModal.open(e.target.cloneNode(), "col-10");
            });
        }
        addClickEvent();

        function findHeight() {
            if (!(window.innerWidth < 762)) {
                const height = $(currentImg).height();
                $("#cars-details-container .image-container").css("height", height + "px");
            }
            else {
                $("#cars-details-container .image-container").css("height", "auto");
            }
        }


        $(window).on("resize", findHeight);
        $(currentImg).one("load", findHeight);
        findHeight();
    }

    function calculatePrice() {
        const carCost = parseFloat($("#cars-details-container input[type=hidden]").val());
        const cashPayment = parseFloat($("#cars-details-container input[type=number]").val());
        const paymentLength = parseInt($("#cars-details-container select").val());

        const montlyPayment = Math.round(carCost / paymentLength * 12 * 1.045 / 12);
        const totalCost = montlyPayment * paymentLength;

        $("#cars-details-container .price-result > p.month").text(montlyPayment + "Kr per månad");
        $("#cars-details-container .price-result > p.total").text("Total pris: " + totalCost + "Kr");
    }

    $("#cars-details-container form").on("submit", function (e) {
        e.preventDefault();
        calculatePrice();
    });
    calculatePrice();

    $("#cars-details-container .share-button").on("click", function (e) {
        const formTemplate = '<form id="shareCarForm"><h4>Dela denna bil!</h4><div class="form-group"><input type="email" placeholder="Email" class="form-control" /></div><div class="form-group"><input type="submit" class="btn btn-primary btn-block" /></div></form>';
        const spinnerTemplate = '<div class="spinner"><div class="bounce1"></div><div class="bounce2"></div><div class="bounce3"></div></div>';

        CustomModal.open(formTemplate, "col-xl-4 col-lg-4 col-md-8 col-sm-10");

        $("#shareCarForm").on("submit", function (e) {
            e.preventDefault();

            const json = JSON.stringify({
                Email: $("#shareCarForm input[type=email]").val(),
                LicenseNumber: window.location.href.split("/").pop()
            });

            CustomModal.changeContent(spinnerTemplate);

            $.ajax({
                method: "POST",
                url: "/api/bil/dela",
                contentType: "application/json; charset=utf-8",
                data: json,
                success: CustomModal.close
            });
        });
    });
});
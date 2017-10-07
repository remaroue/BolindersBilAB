$(function(){

    $('#removeCars').on('click', function () {
        if (confirm('Bekräfta borttagning'))
        {
            $('#carListTable').submit();
        }
    });

    $("#editBrand").val("");
    $("#editBrand").on("change", function (e) {
        location.href = $("#editBrand").val();
    });

    $(".update-car").on("click", function (e) {
        if (confirm('Bekräfta uppdatering')) {
            var licenseNumber = $(e.target).data("licensenumber");

            $.post("/admin/bil/pop/" + licenseNumber, function (e) {
                location.reload();
            });
        }
    });
});
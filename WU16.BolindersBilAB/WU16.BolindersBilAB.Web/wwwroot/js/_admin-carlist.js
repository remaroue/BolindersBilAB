$(function(){

    $('#removeCars').on('click', function () {
        if (confirm('Bekräfta borttagning'))
        {
            $('#carListTable').submit();
        }
    });
});
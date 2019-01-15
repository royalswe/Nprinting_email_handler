function webgridAdmin() {
    $('thead tr:first th:last').append('<em class="glyphicon glyphicon-cog"></em>');
    $('thead tr:first th a').append('<em class="glyphicon glyphicon-sort"></em>');

    $("tfoot a").addClass('btn btn-default'); // add bootstrap pagination

    $("tfoot td") // bootstrap blue button
        .contents()
        .filter(function () {
            if (this.nodeType === 3 && this.length > 1) {
                return this.nodeType
            }
        })
        .wrap('<span class="btn btn-primary" />');
}

webgridAdmin();

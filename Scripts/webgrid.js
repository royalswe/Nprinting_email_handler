$(document).ready(function () {
    $(".show-create-form").click(function () {
        $('.create-user-form').toggle('fast');
    });

    // Delete user
    $('#confirm-delete').on('click', '.btn-ok', function (e) {
        var id = $(this).data('recordId');
        var tr = $("[id=in_ID][value=" + id + "]").parent().parent(); // get the current tr
        console.log('deleted');
        var isSuccess = -1;
        var $modalDiv = $(e.delegateTarget);
        var id = $(this).data('recordId');
        var url = $(this).data('url');
        $modalDiv.modal('hide');

        $.ajax({
            url: url + '/' + id,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',

            success: function (result) {
                isSuccess = result;
            },
            error: function (result) {
                isSuccess = result;
            }

        }).done(function () {
            if (isSuccess === "1") {
                tr.remove();
                FlashMessage("Mottagaren borttagen", "success");
            }
            else if (isSuccess === "0") {
                FlashMessage("Mottagaren finns inte", "error", 4000);
            }
            else {
                FlashMessage(isSuccess, "warning", 8000);
            }
        });

    });
    // Show confirm dialog
    $('#confirm-delete').on('show.bs.modal', function (e) {
        var data = $(e.relatedTarget).data();
        $('.title', this).text(data.recordTitle);
        $('.btn-ok', this).data('recordId', data.recordId);
        $('.btn-ok', this).data('url', data.url);
    });
});

function webgrid() {
    $('.modify').hide();  // default is display
    $('thead tr:first th:first').append('<em class="glyphicon glyphicon-cog"></em>');
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

    $('.edit-btn, .cancel-btn').on("click", function () {
        var tr = $(this).parents('tr:first');
        tr.find('.modify, .display').toggle();
        return false;
    });

    // Update user
    $('.save-btn').on("click", function () {
        var isSuccess = -1;
        var tr = $(this).parents('tr:first');

        var id = tr.find("#in_ID").val();
        var unit = tr.find("#in_unit").val();
        var name = tr.find("#in_name").val();
        var email = tr.find("#in_email").val();
        var password = tr.find("#in_password").val();
        var role = tr.find("#in_role").val();
        var url = $(this).data('url');
 
        console.log('update')

        var User =
        {
            "ID": id,
            "SchoolUnit": unit,
            "Name": name,
            "Email": email,
            "Password": password,
            "Role": role     
        };

        $.ajax({
            url: url,
            data: JSON.stringify(User),
            type: 'POST',
            contentType: 'application/json; charset=utf-8',

            success: function (result) {
                isSuccess = result;
            },
            error: function (result) {
                isSuccess = result;
            }

        }).done(function () {
            if (isSuccess === "1") {
                tr.find('.modify, .display').toggle();
                FlashMessage("Mottagaren har uppdaterats", "success");
                tr.find("#lbl_unit").text(unit);
                tr.find("#lbl_name").text(name);
                tr.find("#lbl_email").text(email);
               // tr.find("#lbl_password").text('Secret');
                tr.find("#lbl_role").text(role);
                $("#in_role select").val(role);
            }
            else if (isSuccess === "0") {            
                FlashMessage("Mottagaren kunde inte uppdateras", "error", 4000);
            }
            else {
                FlashMessage(isSuccess, "warning", 8000);
            }
        });
        return false;
    });

}

webgrid();
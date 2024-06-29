const baseApiUri = '<API BASE URL>';
const mapApiKey = '<GOOGLE MAPS APIKEY>'

$(document).ready(function() {

    getContacts();

    /* We use 1s timeout to perform search so that we don't hit the API too soon */
    var timeout = null;
    $('#searchContacts').on('input', function() {
        if (timeout) {
            clearTimeout(timeout);
        }
        timeout = setTimeout(() => {
            var inputValue = $(this).val();
            if (inputValue.length >= 3) {
                getContacts(inputValue);
            }
            else {
                getContacts();
            }
        }, 1000);
      });

    $('#createContactBtn').click(function() {
        $('#contactForm').show();
        $('#backdrop').show();
        $('#contactId').val('');
        $('#deleteBtn').hide();
    });

    $('#backdrop').click(function() {
        resetFields();
        $('#contactForm').hide();
        $('#deleteConfirmationForm').hide();
        $(this).hide();
    });

    $('#cancelBtn').click(function() {
        $('#contactForm').hide();
        $('#backdrop').hide();

        resetFields();
    });

    $('#deleteCancelBtn').click(function() {
        $('#deleteConfirmationForm').hide();
    });

    $('#deleteBtn').click(function() {
        $('#deleteConfirmationForm').show();
    });

    $('#contactFormForm').on('submit', function(e) {
        e.preventDefault();

        const isUpdate = $('#contactId').val() !== '' ? true : false;

        var formData = {
            id: isUpdate ? $('#contactId').val() : 0,
            name: $('#name').val(),
            email: $('#email').val(),
            phone: $('#phone').val(),
            address: $('#address').val()
        };

        $.ajax({
            url: isUpdate ? `${baseApiUri}/contact/${formData.id}` : `${baseApiUri}/contact`,
            type: isUpdate ? 'PUT' : 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function(contact) {
                if (isUpdate) {
                    var row = $(`#contact${contact.id}`);
                    row.empty(); 
                    row.append($('<td>').text(contact.id));
                    row.append($('<td>').text(contact.name));
                    row.append($('<td>').text(contact.email));
                    row.append($('<td>').text(contact.phone));
                    row.append($('<td>').html(`<a href="javascript: viewContact(${contact.id})">View</a>`));

                    row = $(`#contact${contact.id}address`);
                    row.empty(); 

                    row.append($('<td>').text(''));
                    row.append($('<td>').text(''));
                    row.append($('<td>').attr('colspan', '3').attr('style', 'text-align:left;').text(contact.address));
                }
                else {
                    const table = $('#contacts-table');
                    addContactRow(table, contact);
                }

                resetFields();

                $('#contactForm').hide();
                $('#backdrop').hide();
            },
            error: function(error) {
                console.error('Error: ', error);
            }
        });
    });

    $('#deleteConfirmationFormForm').on('submit', function(e) {
        e.preventDefault();

        const contactId = $('#deleteContactId').val();

        $.ajax({
            url: `${baseApiUri}/contact/${contactId}`,
            type: 'DELETE',
            contentType: 'application/json',
            success: function(result) {
                if (result) {
                    var row = $(`#contact${contactId}`)
                    row.remove();
                    row = $(`#contact${contactId}address`)
                    row.remove();
                    
                    resetFields();

                    $('#deleteConfirmationForm').hide();
                    $('#contactForm').hide();
                    $('#backdrop').hide();
                }
            },
            error: function(error) {
                console.error('Error: ', error);
            }
        });
    });
});

function resetFields() {
    $('#name').val('');
    $('#email').val('');
    $('#phone').val('');
    $('#address').val('');
    $('#contactId').val('');
    $('#deleteContactId').val('');
    $('#contactName').text('');
    $('#addressMap').attr('src', '');
    $('#addressMap').hide();
}

function getContacts(q) {
    if (!q) {
        q = ''
    }
    $.ajax({
        url: `${baseApiUri}/contacts?q=${q}`,
        type: 'GET',
        success: function(data) {
            var table = $('<table>').attr('id', 'contacts-table').addClass('table table-striped table-bordered');

            /* The header of the table */
            var header = $('<tr>');
            header.append($('<th>').text('Id'));
            header.append($('<th>').text('Name'));
            header.append($('<th>').text('Email'));
            header.append($('<th>').text('Phone'));
            header.append($('<th>').html('&nbsp;'));
            table.append(header);

            $.each(data, function(index, contact) {
                addContactRow(table, contact);
            });

            $('#contacts').empty();
            $('#contacts').append(table);
        },
        error: function(error) {
            console.log('Error: ', error);
        }
    });
}

function addContactRow(table, contact) {
    var row = $('<tr>').attr('id', `contact${contact.id}`);
    row.append($('<td>').text(contact.id));
    row.append($('<td>').text(contact.name));
    row.append($('<td>').text(contact.email));
    row.append($('<td>').text(contact.phone));
    row.append($('<td>').html(`<a href="javascript: viewContact(${contact.id})">View</a>`));
    table.append(row);
    row = $('<tr>').attr('id', `contact${contact.id}address`);
    row.append($('<td>').text(''));
    row.append($('<td>').text(''));
    row.append($('<td>').attr('colspan', '3').attr('style', 'text-align:left;').text(contact.address));
    table.append(row);
}

function viewContact(id) {
    $.ajax({
        url: `${baseApiUri}/contact/${id}`,
        type: 'GET',
        success: function(contact) {
            $('#contactForm').show();
            $('#backdrop').show();
            $('#deleteBtn').show();

            // Fill-up form values
            $('#name').val(contact.name);
            $('#email').val(contact.email);
            $('#phone').val(contact.phone);
            $('#address').val(contact.address);
            $('#contactId').val(id);

            // We set name & id for delete confirmation
            $('#contactName').text(contact.name);
            $('#deleteContactId').val(id);

            if (contact.address) {
                // We use Google Maps API to get lat & lon
                $.ajax({
                    url: 'https://maps.googleapis.com/maps/api/geocode/json',
                    data: {
                      address: contact.address,
                      key: mapApiKey
                    },
                    success: function(response) {
                        if (response && response.results[0] && response.results[0].geometry) {
                            var location = response.results[0].geometry.location;
                            $('#addressMap').attr('src', `https://maps.googleapis.com/maps/api/staticmap?center=${contact.address}&zoom=13&size=550x250&maptype=roadmap&markers=color:red%7Clabel:A%7C${location.lat},${location.lng}&key=${mapApiKey}`);
                            $('#addressMap').show();
                        }
                        else {
                            $('#addressMap').hide();
                        }
                    }
                });
            }
            else {
                $('#addressMap').hide();
            }
        },
        error: function(error) {
            console.log('Error: ', error);
        }
    });
}
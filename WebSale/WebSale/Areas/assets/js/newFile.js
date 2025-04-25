$(document).ready(function() {
    window.onload = function() {
        CKEDITOR.replace('moTa');
    };
    function updateOrderStatus(orderId, status) {
        $.ajax({
            url: "/Admin/Order/changeStatusOrder",
            type: "POST",
            dataType: "json",
            data: {
                orderId: orderId,
                status: status
            },
            success: function(response) {
                if (response.isSuccess) {
                    Swal.fire({
                        title: 'Th�nh c�ng!',
                        text: response.message,
                        icon: 'success',
                        confirmButtonText: 'OK',
                        timer: 3000,
                        timerProgressBar: true,
                    }).then(() => {
                        window.location.href = '/Admin/Order';
                    });
                }
            },
            error: function() {
                Swal.fire({
                    title: 'L?i!',
                    text: 'Kh�ng th? c?p nh?t tr?ng th�i ??n h�ng. Vui l�ng th? l?i sau.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    }

    // Handle change status button click
    $(document).on("click", ".btn-ChangeStatus", function(e) {
        e.preventDefault();
        var orderId = $(this).data('order'); // L?y order ID t? data attribute c?a n�t
        console.log("Order ID:", orderId); // Ki?m tra xem Order ID ???c l?y ?�ng hay kh�ng

        Swal.fire({
            title: 'C?p nh?t tr?ng th�i ??n h�ng',
            text: 'Ch?n tr?ng th�i b?n mu?n c?p nh?t:',
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: '?ang giao h�ng',
            denyButtonText: '?� giao',
            cancelButtonText: 'H?y ??n',
            showCloseButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                updateOrderStatus(orderId, 'dang_giao');
            } else if (result.isDenied) {
                updateOrderStatus(orderId, 'da_giao');
            } else if (result.isDismissed && result.dismiss === Swal.DismissReason.cancel) {
                updateOrderStatus(orderId, 'huy_don');
            }
        });
    });
    $(document).on("click", ".btn-delete-brand", function(e) {
        e.preventDefault();
        var id = $(this).data('productid');
        var row = $(this).closest('tr');
        Swal.fire({
            title: 'B?n c� mu?n x�a nh�n h�ng',
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'X�a',
            cancelButtonText: 'H?y',
            showCloseButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Admin/Brand/deleteBrand",
                    type: "POST",
                    dataType: "json",
                    data: {
                        id: id,
                    },
                    success: function(response) {
                        if (response.isSuccess) {
                            Swal.fire({
                                title: 'X�a th�nh c�ng!',
                                text: response.message,
                                icon: 'success',
                                confirmButtonText: 'OK',
                                timer: 3000,
                                timerProgressBar: true,
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.href = "/Admin/Brand/";
                                }
                            });
                        }
                    },
                    error: function() {
                        Swal.fire({
                            title: 'L?i!',
                            text: 'Kh�ng th? c?p nh?t tr?ng th�i ??n h�ng. Vui l�ng th? l?i sau.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                });
            }
        });
    });
});

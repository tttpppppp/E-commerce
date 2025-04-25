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
                        title: 'Thành công!',
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
                    text: 'Không th? c?p nh?t tr?ng thái ??n hàng. Vui lòng th? l?i sau.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    }

    // Handle change status button click
    $(document).on("click", ".btn-ChangeStatus", function(e) {
        e.preventDefault();
        var orderId = $(this).data('order'); // L?y order ID t? data attribute c?a nút
        console.log("Order ID:", orderId); // Ki?m tra xem Order ID ???c l?y ?úng hay không

        Swal.fire({
            title: 'C?p nh?t tr?ng thái ??n hàng',
            text: 'Ch?n tr?ng thái b?n mu?n c?p nh?t:',
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: '?ang giao hàng',
            denyButtonText: '?ã giao',
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
            title: 'B?n có mu?n xóa nhãn hàng',
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Xóa',
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
                                title: 'Xóa thành công!',
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
                            text: 'Không th? c?p nh?t tr?ng thái ??n hàng. Vui lòng th? l?i sau.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                });
            }
        });
    });
});

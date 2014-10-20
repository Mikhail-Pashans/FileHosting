(function ($) {
    var $fileId = $('#fileId');
    var $tagsElement = $('#fileTags');
    var $descriptionElement = $('#fileDescription');
    var $commentsPanel = $('#commentsPanel');
    var $cancelButton = $('.btn-cancel');

    var $deleteFileButton = $('.btn-delete-file');
    var $deleteFileForm = $('#deleteFileForm');
    var $modal = $('#deleteConfirmation');
    var $confirmButton = $('.btn-confirm');    


    var fileId = $fileId.text();
    var tagsValue = $tagsElement.val();
    var descriptionValue = $descriptionElement.val();

    $cancelButton.on('click', function (event) {
        event.preventDefault();
        $tagsElement.val(tagsValue);
        $descriptionElement.val(descriptionValue);
    });

    $deleteFileButton.on('click', function (event) {
        event.preventDefault();

        $modal.modal('show');

        $confirmButton.on('click', function (innerEvent) {
            innerEvent.preventDefault();
            $deleteFileForm.submit();
            $modal.modal('hide');
        });
    });    

    var getParameterByName = function(url, name) {
        var match = new RegExp('[?&]' + name + '=([^&]*)').exec(url);
        return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
    };

    var loadComments = function(page, messageType) {
        $commentsPanel.load(
            '/Files/GetCommentsForFile',
            {
                fileId: fileId,
                page: page,
                messageType: messageType
            },
            function() {                
                var $pagination = $('.pagination');
                var $newCommentForm = $('#newCommentForm');                    

                $commentsPanel.on('change', '.activate-comment-checkbox', function (event) {
                    event.preventDefault();                    
                    var $checkbox = $(this);
                    var commentId = $checkbox.val();
                    var isActive = $checkbox.prop('checked');
                    $.post(
                        '/Files/ChangeCommentState',
                        {
                            fileId: fileId,
                            commentId: commentId,
                            isActive: isActive
                        },
                        function(response) {
                            if (response) {
                                var $panel = $checkbox.parent().parent().parent().parent();
                                if (isActive) {
                                    $panel.removeClass('panel-danger');
                                    $panel.addClass('panel-info');
                                } else {
                                    $panel.removeClass('panel-info');
                                    $panel.addClass('panel-danger');
                                }
                            }
                        }
                    );
                });

                $commentsPanel.on('submit', '.delete-comment-form', function(event) {
                    event.preventDefault();
                    var $form = $(this);

                    var modalHtml = $modal.html();

                    $('.modal-title', $modal).text('Delete comment confirmation');
                    $('.modal-body', $modal).text('Are you sure you want to delete this comment?');

                    $modal.modal('show');

                    $confirmButton.on('click', function(innerEvent) {
                        innerEvent.preventDefault();
                        $modal.modal('hide');
                        $.post(
                            $form.attr('action'),
                            $form.serialize(),
                            function (response) {
                                loadComments(null, response);
                                $modal.html(modalHtml);
                            }
                        );
                    });
                });

                //$commentsPanel.on('click', '.btn-delete-comment', function(event) {
                //    event.preventDefault();
                //    var $button = $(this);
                //    var $form = $button.parent();

                //    $modalComment.modal('show');

                //    $confirmCommentButton.on('click', function (innerEvent) {
                //        innerEvent.preventDefault();
                //        $form.submit();
                //        $modalComment.modal('hide');
                //    });
                //});

                $newCommentForm.on('submit', function(event) {
                    event.preventDefault();
                    var $form = $(this);
                    $.post(
                        $form.attr('action'),
                        $form.serialize(),
                        function(response) {
                            loadComments(null, response);
                        }
                    );
                });

                $pagination.on('click', 'a', function(event) {
                    event.preventDefault();
                    var href = $(this).attr('href');
                    var pageNumber = getParameterByName(href, 'page');
                    loadComments(pageNumber, null);
                });
            }
        );
    };

    loadComments(null, null);
})(jQuery);
(function ($) {
    var $fileId = $('input[name=fileId]');
    var $fileTags = $('#fileTags');
    var $fileDescription = $('#fileDescription');
    var $commentsPanel = $('#commentsPanel');
    var $cancelButton = $('.btn-cancel');
    var $deleteFileButton = $('.btn-delete-file');
    var $deleteFileForm = $('#deleteFileForm');     


    var fileId = $fileId.val();
    var tagsValue = $fileTags.val();
    var descriptionValue = $fileDescription.val();

    $cancelButton.on('click', function (event) {
        event.preventDefault();
        $fileTags.val(tagsValue);
        $fileDescription.val(descriptionValue);
    });

    $deleteFileButton.on('click', function (event) {
        event.preventDefault();
        $deleteFileForm.submit();
    });

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
            function (response) {
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

    $commentsPanel.on('submit', '.delete-comment-form', function (event) {
        event.preventDefault();
        var $form = $(this);
        $.post(
            $form.attr('action'),
            $form.serialize(),
            function (response) {
                loadComments(null, response);
            }
        );
    });

    var getParameterByName = function (url, name) {
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
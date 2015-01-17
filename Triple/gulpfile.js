var gulp = require('gulp');
var changed = require('gulp-changed');
var concat = require('gulp-concat');
var stripDebug = require('gulp-strip-debug');
var uglify = require('gulp-uglify');
var minifyHTML = require('gulp-minify-html');
var autoprefix = require('gulp-autoprefixer');
var minifyCSS = require('gulp-minify-css');
var rename = require('gulp-rename');

gulp.task('bundle-scripts', function() {
    var jsPath = { jsSrc: ['./app/app.js', './app/**/*.js'], jsDest: './appbuild' };
    gulp.src(jsPath.jsSrc)
        .pipe(concat('ngscripts.js'))
        .pipe(stripDebug())
        //.pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(jsPath.jsDest));
});

gulp.task('minify-html', function() {
    var opts = { empty: true, quotes: true };
    var htmlPath = { htmlSrc: './app/views/*.html', htmlDest: './appbuild/views' };

    return gulp.src(htmlPath.htmlSrc)
        .pipe(changed(htmlPath.htmlDest))
        .pipe(minifyHTML(opts))
        .pipe(gulp.dest(htmlPath.htmlDest));
});

gulp.task('minify-css', function() {
    var cssPath = { cssSrc: ['./content/css/*.css', '!*.min.css', '!/**/*.min.css'], cssDest: './contentbuild/css/' };

    return gulp.src(cssPath.cssSrc)
        .pipe(concat('styles.css'))
        .pipe(autoprefix('last 2 versions'))
        .pipe(minifyCSS())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(cssPath.cssDest));
});

// default gulp task
gulp.task('default', ['minify-html', 'bundle-scripts', 'minify-css'], function() {
    // watch for HTML changes
    gulp.watch('./app/views/*.html', ['minify-html']);
    // watch for JS changes
    gulp.watch('./app/**/*.js', ['bundle-scripts']);
    // watch for CSS changes
    gulp.watch('./content/css/*.css', ['minify-css']);
});
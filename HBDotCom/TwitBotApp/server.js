'use strict';
var http = require('http');
var port = process.env.PORT || 1337;
var Twit = require('twit');
var config = require('./config.js');

const T = new Twit(config);

http.createServer(function (req, res) {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end("Listening");
}).listen(port);

// Setting up a user stream
var stream = T.stream('user');

// Now looking for tweet events
stream.on('tweet', tweetEvent);

stream.on('')

// Here a tweet event is triggered!
function tweetEvent(tweet) {

    // Who is mentioned?
    var mentions = tweet.entities.user_mentions.select(x => x.screen_name);

    if (mentions.includes("AppHarleys")) {
        console.log("I got mentioned");

        searchAndReply(tweet);
    }

}

function searchAndReply(tweet) {

    var name = tweet.user.screen_name;

    var threadOriginal = getTopParent(tweet);

    var duplicateCount = getIdenticalTweetCount(threadOriginal);

    var reply = `@${name} `;

    switch (duplicateCount) {
        case 100:
            reply += `This tweet has been done 100 or more times before. Wowzers!`;
            break;
        case 0:
            reply += 'Looks unique.';
            break;
        default:
            reply += `This tweet has been done ${duplicateCount} times before.`;
    }

    postReply(reply);
}

function postReply(reply) {
    T.post('statuses/update', { status: reply, in_reply_to_status_id: tweet.id }, tweeted);
}

function getIdenticalTweetCount(tweet) {
    var tweets = T.get('search/tweets', { q: searchText, max_id: threadStarter.id, count: 100 });
    return tweets.length;
}


// Recurse until the original tweet is found
function getTopParent(tweet) {
    if (tweet.in_reply_to_status_id) {
        const parentTweet = T.get('statuses/show', { id: tweet.in_reply_to_status_id });
        getFirstInThread(parentTweet);
    }
    else {
        return tweet;
    }
}

function tweeted(err, reply) {
    if (err !== undefined) {
        console.log(err);
        Console.WriteLine(err);
    } else {
        console.log('Tweeted: ' + reply);
        Console.WriteLine('Tweeted: ' + reply);
    }
}
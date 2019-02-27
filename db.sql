CREATE DATABASE IF NOT EXISTS  Synchro;

USE Synchro;

CREATE TABLE IF NOT EXISTS players (
    id INT unsigned NOT NULL AUTO_INCREMENT,
    name varchar(20) UNIQUE NOT NULL,
    games_played INT DEFAULT 0,
    suggested_difficulty varchar(10) NOT NULL DEFAULT "easy",
    difficulty_ctr INT DEFAULT 0,
    laser_training BOOLEAN DEFAULT False,
    gesture_training BOOLEAN DEFAULT False,
    unscramble_training BOOLEAN DEFAULT False,
    trivia_training BOOLEAN DEFAULT False,
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS games (
    game_id INT unsigned NOT NULL AUTO_INCREMENT,
    player INT unsigned,
    player_game_idx INT unsigned DEFAULT 0,
    difficulty varchar(10) NOT NULL,
    gestures_acc FLOAT(4,3) DEFAULT -1,
    gestures_timer_avg FLOAT(4,2) DEFAULT -1,
    unscramble_acc FLOAT(4,3) DEFAULT -1,
    unscramble_timer_avg FLOAT(4,2) DEFAULT -1,
    trivia_acc FLOAT(4,3) DEFAULT -1,
    trivia_timer_avg FLOAT(4,2) DEFAULT -1,
    lives_left INT unsigned Default 3,
    died BOOLEAN DEFAULT False,
    total_score FLOAT(6,2) DEFAULT 0,
    PRIMARY KEY(game_id),
    FOREIGN KEY (player) REFERENCES players(id)
);

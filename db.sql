CREATE DATABASE IF NOT EXISTS  Synchro;

USE Synchro;

CREATE TABLE IF NOT EXISTS players (
    id INT unsigned NOT NULL AUTO_INCREMENT,
    name varchar(20) NOT NULL,
    games_played INT DEFAULT 0,
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS games (
    game_id INT unsigned NOT NULL AUTO_INCREMENT,
    player INT unsigned,
    player_game_idx INT unsigned DEFAULT 0,
    gestures_pass INT unsigned DEFAULT 0,
    gestures_fail INT unsigned DEFAULT 0,
    speech_pass INT unsigned DEFAULT 0,
    speech_fail INT unsigned DEFAULT 0,
    n_hits INT unsigned DEFAULT 0,
    PRIMARY KEY(game_id),
    FOREIGN KEY (player) REFERENCES players(id)
);

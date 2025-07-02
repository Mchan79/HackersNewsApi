# Hacker News Best Stories API

## Overview

This project provides a RESTful API to fetch the top N best stories from the Hacker News API based on their score.

## Features

- Retrieves best story IDs from Hacker News API
- Fetches and caches story details
- Sorts by score in descending order
- API endpoint: `/api/stories?n=10`

## Setup Instructions

1. Clone the repository.
2. Run `dotnet restore` in both projects.
3. Start the API project:
4. Test with the console app (includes logic to rate limiting):

## Assumptions

- Responses are cached for 5 minutes to reduce API load.

## Enhancements

- Add paging and filtering.
- Add retry logic for failed API calls.

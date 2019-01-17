# -*- coding: utf-8 -*-
# Version 0.0.1

'''
Emits event data to be used by a Katalye server.
'''

# Import python libs
from __future__ import absolute_import, print_function, with_statement, unicode_literals
import logging

import salt.returners
import salt.utils.http
import salt.utils.json

log = logging.getLogger(__name__)

# Define the module's virtual name
__virtualname__ = 'katalye'

def __virtual__():
    return __virtualname__

def _get_options(ret):
    '''
    Returns options used for Katalye.
    '''
    defaults = {'server': '', 'secure': False, 'path_prefix': '/api/v1/export/event'}
    attrs = {'server': 'server', 'secure': 'secure', 'path_prefix': 'path_prefix'}
    _options = salt.returners.get_returner_options(__virtualname__,
                                                   ret,
                                                   attrs,
                                                   __salt__=__salt__,
                                                   __opts__=__opts__,
                                                   defaults=defaults)

    return _options

def _post_data(options=None, tag='', json=None):
    '''
    Post data endpoint
    '''

    schema = 'https' if options['secure'] else 'http'
    endpoint = schema + '://' + options['server'] + options['path_prefix']
    base_url = endpoint if endpoint.endswith('/') else endpoint + "/"

    log.error(
      tag + " " + salt.utils.json.dumps(json),
    )

    res = salt.utils.http.query(
        url=base_url + tag,
        method='POST',
        params={},
        data=json,
        decode=False,
        status=True,
        header_dict={'Content-Type':'application/json'},
        opts=__opts__,
    )

    if res.get('status', None) == salt.ext.six.moves.http_client.OK:
        return True
    else:
        log.error(
            'Error returned from endpoint: %s.',
            salt.utils.json.dumps(res)
        )
        return False

def returner(ret):
    '''
    Not currently used.
    '''
    pass

def event_return(events):
    '''
    Write event data (return data and non-return data) to Katalye
    '''
    if len(events) == 0:
        # events is an empty list.
        return
    opts = _get_options({})  # Pass in empty ret, since this is a list of events

    for event in events:
        tag = event.get('tag', '')
        data = event.get('data', '')
        res = _post_data(options=opts,tag=tag, json=salt.utils.json.dumps(data))
        if not res:
            log.error('Could not write event to Katalye: %s.', opts['server'])
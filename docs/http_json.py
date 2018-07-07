# -*- coding: utf-8 -*-
'''
Take data from salt and "return" it into http endpoint as json

Add the following to the minion or master configuration file.

.. code-block:: yaml

    http_json.endpoint: <endpoint>

Common use is to log all events on the master. This can generate a lot of
noise, so you may wish to configure batch processing and/or configure the
:conf_master:`event_return_whitelist` or :conf_master:`event_return_blacklist`
to restrict the events that are written.
'''

# Import python libs
from __future__ import absolute_import, print_function, with_statement, unicode_literals
import logging

import salt.returners
import salt.utils.http
import salt.utils.json

log = logging.getLogger(__name__)

# Define the module's virtual name
__virtualname__ = 'http_json'


def __virtual__():
    return __virtualname__


def _get_options(ret):
    '''
    Returns options used for the rawfile_json returner.
    '''
    defaults = {'endpoint': ''}
    attrs = {'endpoint': 'endpoint'}
    _options = salt.returners.get_returner_options(__virtualname__,
                                                   ret,
                                                   attrs,
                                                   __salt__=__salt__,
                                                   __opts__=__opts__,
                                                   defaults=defaults)

    return _options

def _post_data(options=None, json=None):
    '''
    Post data endpoint
    '''

    res = salt.utils.http.query(
        url=options['endpoint'],
        method='POST',
        params={},
        data=json,
        decode=True,
        status=True,
        header_dict={},
        opts=__opts__,
    )

    if res.get('status', None) == salt.ext.six.moves.http_client.OK:
        return True
    else:
        log.error(
            'Error returned from endpoint. Status code: %s.',
            res.status_code
        )
        return False

def returner(ret):
    '''
    Write the return data to http endpoint
    '''
    opts = _get_options({})  # Pass in empty ret, since this is a list of events
    res = _post_data(options=_options, json=salt.utils.json.loads(ret))

    return res

def event_return(events):
    '''
    Write event data (return data and non-return data) to http endpoint
    '''
    if len(events) == 0:
        # events is an empty list.
        return
    opts = _get_options({})  # Pass in empty ret, since this is a list of events
    
    for event in events:
        res = _post_data(options=opts, json=salt.utils.json.loads(event))
        if not res:
            log.error('Could not write event to http_json %s', opts['endpoint'])
            raise